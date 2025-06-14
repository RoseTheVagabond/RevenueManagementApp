-- Created by Vertabelo (http://vertabelo.com)
-- Last modification date: 2025-06-14 13:52:48.955

-- foreign keys
ALTER TABLE Company DROP CONSTRAINT Company_Client;

ALTER TABLE Contract DROP CONSTRAINT Contract_Client;

ALTER TABLE Contract DROP CONSTRAINT Contract_Discount;

ALTER TABLE Contract DROP CONSTRAINT Contract_Software;

ALTER TABLE Individual DROP CONSTRAINT Individual_Client;

ALTER TABLE Software DROP CONSTRAINT Software_Cathegory;

-- tables
DROP TABLE Cathegory;

DROP TABLE Client;

DROP TABLE Company;

DROP TABLE Contract;

DROP TABLE Discount;

DROP TABLE Individual;

DROP TABLE Software;

-- End of file.

