-- Created by Vertabelo (http://vertabelo.com)
-- Last modification date: 2025-06-14 13:52:48.955

-- tables
-- Table: Cathegory
CREATE TABLE Cathegory (
    id int  NOT NULL,
    name nvarchar(20)  NOT NULL,
    CONSTRAINT Cathegory_pk PRIMARY KEY  (id)
);

-- Table: Client
CREATE TABLE Client (
    id int  NOT NULL,
    clientType nvarchar(10)  NOT NULL,
    CONSTRAINT Client_pk PRIMARY KEY  (id)
);

-- Table: Company
CREATE TABLE Company (
    Client_id int  NOT NULL,
    KRS nvarchar(10)  NOT NULL,
    name nvarchar(100)  NOT NULL,
    address nvarchar(200)  NOT NULL,
    email nvarchar(100)  NOT NULL,
    phoneNumber nvarchar(14)  NOT NULL,
    CONSTRAINT Company_pk PRIMARY KEY  (Client_id)
);

-- Table: Contract
CREATE TABLE Contract (
    id int  NOT NULL,
    start datetime  NOT NULL,
    "end" datetime  NULL,
    softwareDeadline datetime  NOT NULL,
    isSigned bit  NOT NULL,
    isPaid bit  NOT NULL,
    toPay decimal(8,2)  NOT NULL,
    paid decimal(8,2)  NOT NULL,
    Client_id int  NOT NULL,
    Software_id int  NOT NULL,
    Discount_id int  NOT NULL,
    CONSTRAINT Contract_pk PRIMARY KEY  (id)
);

-- Table: Discount
CREATE TABLE Discount (
    id int  NOT NULL,
    percentage int  NOT NULL,
    start datetime  NOT NULL,
    "end" datetime  NOT NULL,
    CONSTRAINT Discount_pk PRIMARY KEY  (id)
);

-- Table: Individual
CREATE TABLE Individual (
    Client_id int  NOT NULL,
    PESEL nvarchar(11)  NOT NULL,
    firstName nvarchar(50)  NOT NULL,
    lastName nvarchar(50)  NOT NULL,
    address nvarchar(200)  NOT NULL,
    email nvarchar(100)  NOT NULL,
    phoneNumber nvarchar(14)  NOT NULL,
    deletedAt datetime  NULL,
    CONSTRAINT Individual_pk PRIMARY KEY  (Client_id)
);

-- Table: Software
CREATE TABLE Software (
    id int  NOT NULL,
    name nvarchar(50)  NOT NULL,
    description nvarchar(200)  NOT NULL,
    currentVersion nvarchar(5)  NOT NULL,
    Cathegory_id int  NOT NULL,
    CONSTRAINT Software_pk PRIMARY KEY  (id)
);

-- foreign keys
-- Reference: Company_Client (table: Company)
ALTER TABLE Company ADD CONSTRAINT Company_Client
    FOREIGN KEY (Client_id)
    REFERENCES Client (id);

-- Reference: Contract_Client (table: Contract)
ALTER TABLE Contract ADD CONSTRAINT Contract_Client
    FOREIGN KEY (Client_id)
    REFERENCES Client (id);

-- Reference: Contract_Discount (table: Contract)
ALTER TABLE Contract ADD CONSTRAINT Contract_Discount
    FOREIGN KEY (Discount_id)
    REFERENCES Discount (id);

-- Reference: Contract_Software (table: Contract)
ALTER TABLE Contract ADD CONSTRAINT Contract_Software
    FOREIGN KEY (Software_id)
    REFERENCES Software (id);

-- Reference: Individual_Client (table: Individual)
ALTER TABLE Individual ADD CONSTRAINT Individual_Client
    FOREIGN KEY (Client_id)
    REFERENCES Client (id);

-- Reference: Software_Cathegory (table: Software)
ALTER TABLE Software ADD CONSTRAINT Software_Cathegory
    FOREIGN KEY (Cathegory_id)
    REFERENCES Cathegory (id);

-- End of file.

