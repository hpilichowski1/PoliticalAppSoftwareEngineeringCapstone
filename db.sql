-- Active: 1762465600144@@127.0.0.1@3307@PoliticalApp
-- Create database
CREATE DATABASE IF NOT EXISTS PoliticalApp
  DEFAULT CHARACTER SET utf8mb4
  DEFAULT COLLATE utf8mb4_0900_ai_ci;
USE PoliticalApp;

CREATE USER 'politicalapp'@'%' IDENTIFIED BY 'PoliticalApp';
GRANT ALL PRIVILEGES ON politicalapp.* TO 'politicalapp'@'%';
FLUSH PRIVILEGES;

-- ---------- Core ----------
CREATE TABLE users (
  user_id    CHAR(36) PRIMARY KEY,
  name       VARCHAR(120) NOT NULL,
  role       ENUM('citizen','representative','candidate','admin') NOT NULL DEFAULT 'citizen',
  password_hash VARCHAR(255) NULL,         -- if you store auth locally
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE representatives (
  rep_id    CHAR(36) PRIMARY KEY,
  user_id   CHAR(36) UNIQUE NULL,          -- optional link to a user account
  name      VARCHAR(120) NOT NULL,
  district  VARCHAR(120) NOT NULL,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_rep_user FOREIGN KEY (user_id) REFERENCES users(user_id)
    ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE TABLE candidates (
  candidate_id  CHAR(36) PRIMARY KEY,
  user_id       CHAR(36) UNIQUE NULL,      -- optional link to a user account
  name          VARCHAR(120) NOT NULL,
  campaign_info JSON NULL,
  created_at    TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_candidate_user FOREIGN KEY (user_id) REFERENCES users(user_id)
    ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB;

CREATE TABLE legislation (
  bill_id  VARCHAR(64) PRIMARY KEY,        -- you used string IDs for bills
  title    VARCHAR(300) NOT NULL,
  summary  TEXT NULL,
  status   ENUM('introduced','committee','floor','passed','failed','withdrawn') NOT NULL DEFAULT 'introduced',
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  INDEX ix_leg_status (status)
) ENGINE=InnoDB;

-- Representative voting history on real bills
CREATE TABLE vote_records (
  vote_id   BIGINT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  rep_id    CHAR(36) NOT NULL,
  bill_id   VARCHAR(64) NOT NULL,
  vote      ENUM('YEA','NAY','ABSTAIN','PRESENT') NOT NULL,
  voted_at  DATETIME NOT NULL,
  UNIQUE KEY ux_rep_bill (rep_id, bill_id),   -- each rep votes once per bill
  CONSTRAINT fk_vr_rep  FOREIGN KEY (rep_id) REFERENCES representatives(rep_id)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT fk_vr_bill FOREIGN KEY (bill_id) REFERENCES legislation(bill_id)
    ON DELETE CASCADE ON UPDATE CASCADE,
  INDEX ix_vr_bill (bill_id, voted_at)
) ENGINE=InnoDB;

-- ---------- Vote Simulation ----------
CREATE TABLE vote_simulations (
  simulation_id CHAR(36) PRIMARY KEY,
  user_id       CHAR(36) NOT NULL,
  bill_id       VARCHAR(64) NOT NULL,
  selected_vote ENUM('YEA','NAY','ABSTAIN','PRESENT') NOT NULL,
  compared_rep_id CHAR(36) NULL,              -- for compareWithRep()
  compare_result  ENUM('MATCH','DIFFER','N/A') NULL,
  created_at    TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_vs_user FOREIGN KEY (user_id) REFERENCES users(user_id)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT fk_vs_bill FOREIGN KEY (bill_id) REFERENCES legislation(bill_id)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT fk_vs_rep  FOREIGN KEY (compared_rep_id) REFERENCES representatives(rep_id)
    ON DELETE SET NULL ON UPDATE CASCADE,
  INDEX ix_vs_user (user_id, created_at),
  INDEX ix_vs_bill (bill_id)
) ENGINE=InnoDB;

-- ---------- Quiz ----------
CREATE TABLE quizzes (
  quiz_id   CHAR(36) PRIMARY KEY,
  title     VARCHAR(200) NOT NULL,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE questions (
  question_id CHAR(36) PRIMARY KEY,
  text        TEXT NOT NULL,
  topic       VARCHAR(120) NULL
) ENGINE=InnoDB;

-- Many-to-many: quiz has multiple questions (ordered)
CREATE TABLE quiz_questions (
  quiz_id     CHAR(36) NOT NULL,
  question_id CHAR(36) NOT NULL,
  position    INT NOT NULL,
  PRIMARY KEY (quiz_id, question_id),
  CONSTRAINT fk_qq_quiz FOREIGN KEY (quiz_id) REFERENCES quizzes(quiz_id)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT fk_qq_q    FOREIGN KEY (question_id) REFERENCES questions(question_id)
    ON DELETE CASCADE ON UPDATE CASCADE,
  INDEX ix_qq_pos (quiz_id, position)
) ENGINE=InnoDB;

-- Optional fixed choices for a question (e.g., Likert or multiple-choice)
CREATE TABLE question_options (
  option_id   CHAR(36) PRIMARY KEY,
  question_id CHAR(36) NOT NULL,
  label       VARCHAR(200) NOT NULL,      -- what the user sees
  value_int   INT NULL,                   -- numeric value to compute alignment
  value_json  JSON NULL,                  -- or structured value
  CONSTRAINT fk_qo_q FOREIGN KEY (question_id) REFERENCES questions(question_id)
    ON DELETE CASCADE ON UPDATE CASCADE,
  INDEX ix_qo_q (question_id)
) ENGINE=InnoDB;

-- A user taking a quiz (for calculateAlignment)
CREATE TABLE user_quiz_attempts (
  attempt_id CHAR(36) PRIMARY KEY,
  user_id    CHAR(36) NOT NULL,
  quiz_id    CHAR(36) NOT NULL,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  alignment_json JSON NULL,               -- e.g., computed match vs reps/candidates
  CONSTRAINT fk_uqa_user FOREIGN KEY (user_id) REFERENCES users(user_id)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT fk_uqa_quiz FOREIGN KEY (quiz_id) REFERENCES quizzes(quiz_id)
    ON DELETE CASCADE ON UPDATE CASCADE,
  INDEX ix_uqa_user (user_id, created_at)
) ENGINE=InnoDB;

CREATE TABLE user_quiz_answers (
  attempt_id  CHAR(36) NOT NULL,
  question_id CHAR(36) NOT NULL,
  option_id   CHAR(36) NULL,              -- chosen option (if MC)
  free_text   TEXT NULL,                  -- or free-form
  value_int   INT NULL,                   -- normalized numeric (e.g., Likert)
  PRIMARY KEY (attempt_id, question_id),
  CONSTRAINT fk_uqan_attempt FOREIGN KEY (attempt_id) REFERENCES user_quiz_attempts(attempt_id)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT fk_uqan_question FOREIGN KEY (question_id) REFERENCES questions(question_id)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT fk_uqan_option FOREIGN KEY (option_id) REFERENCES question_options(option_id)
    ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB;

-- ---------- News ----------
CREATE TABLE news_items (
  news_id  CHAR(36) PRIMARY KEY,
  source   VARCHAR(160) NOT NULL,
  url      VARCHAR(512) NOT NULL,
  headline VARCHAR(512) NOT NULL,
  published_at DATETIME NULL,
  raw_json JSON NULL,                     -- optional payload from API
  UNIQUE KEY ux_news_url (url)
) ENGINE=InnoDB;

-- ---------- CivicHub ----------
-- One CivicHub per user (acts like a profile/dashboard)
CREATE TABLE civic_hubs (
  user_id CHAR(36) PRIMARY KEY,
  updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  CONSTRAINT fk_ch_user FOREIGN KEY (user_id) REFERENCES users(user_id)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;

-- CivicHub.alignedReps
CREATE TABLE civic_hub_aligned_reps (
  user_id CHAR(36) NOT NULL,
  rep_id  CHAR(36) NOT NULL,
  rank_pos    INT NULL, 
  score   DECIMAL(5,2) NULL,
  PRIMARY KEY (user_id, rep_id),
  CONSTRAINT fk_char_user FOREIGN KEY (user_id) REFERENCES civic_hubs(user_id)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT fk_char_rep FOREIGN KEY (rep_id) REFERENCES representatives(rep_id)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;

-- CivicHub.curatedNews
CREATE TABLE civic_hub_curated_news (
  user_id CHAR(36) NOT NULL,
  news_id CHAR(36) NOT NULL,
  score   DECIMAL(6,3) NULL,              -- relevance score
  PRIMARY KEY (user_id, news_id),
  CONSTRAINT fk_chcn_user FOREIGN KEY (user_id) REFERENCES civic_hubs(user_id)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT fk_chcn_news FOREIGN KEY (news_id) REFERENCES news_items(news_id)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;
