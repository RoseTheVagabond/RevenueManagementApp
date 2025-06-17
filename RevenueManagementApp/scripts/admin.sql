SELECT * FROM AspNetRoles;
SELECT * FROM AspNetUsers WHERE UserName = 'admin';
SELECT * FROM AspNetUsers;

DELETE FROM AspNetUserRoles;
DELETE FROM AspNetUsers;
DELETE FROM AspNetRoles;

DELETE FROM Individual WHERE firstName = 'Anna';