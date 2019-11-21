create table _CARD_(
	id uniqueidentifier not null primary key,
	card_num nchar(16) not null,
	cvv nchar(3) not null,
	expires date not null, 
	card_type nvarchar(100) not null,
	card_holder nvarchar(100) not null
)

create table _ADDRESS_(
	id uniqueidentifier not null primary key,
	city nvarchar(100) not null,
	_state_ nvarchar(100) not null,
	country nvarchar(100) not null,
	addressLine1 nvarchar(100) not null,
	addressLine2 nvarchar(100) null
)

create table SHIPPINGCOMPANY (
	id uniqueidentifier not null primary key,
	[name] nvarchar(100) not null
)

create table DISTRIBUTER (
	id uniqueidentifier not null primary key,
	[name] nvarchar(100) not null,
	addressId uniqueidentifier not null references _ADDRESS_(id),
)

create table PRODUCT (
	id uniqueidentifier not null primary key,
	[name] nvarchar(100) not null,
	cost float not null,
	inventory int null, --the reason we do integers is because we do not repackage contents from distributers and cannot have partial products
	minAgeRestrictionInYears int not null, -- we are making the value not null because then we can check if minAgeRestricitonInYears is 0 it is an unrestricted product
	distributerId uniqueidentifier not null references DISTRIBUTER(id)
)

create table CUSTOMER (
	id uniqueidentifier not null primary key,
	[name] nvarchar(200), -- the reason we are using a single name column is because we never address a customer by their first or second name independantly and allows the user to have more than their first and last names in the column
	email nvarchar(100),
	DOB date,
	--shopping_cart uniqueidentifier references SHOPPINGCART(customer,product),
	username nvarchar(100),
	passwordSalt nvarchar(100),
	encryptedPassword nvarchar(100), --hash(hash(id|{password})|passwordSalt) --to check select (id,passwordSalt,encryptedPassword) from CUSTOMER where username = @username
	shipping_address uniqueidentifier references _ADDRESS_(id),
	billing_address uniqueidentifier references _ADDRESS_(id),
	constraint idx_CUSTOMER_username unique clustered (username)
) 

create table ORDERLINE (	--A single line of an order
	id uniqueidentifier not null primary key,
	productQuanitity int not null,
	productId uniqueidentifier not null references PRODUCT(id),
	lineCost float not null
)

create table _ORDER_ (		--A purchased order includs a tracking # and arrival date whereas a pending order would not contain those things
	orderID uniqueidentifier not null primary key,
	orderLineId uniqueidentifier not null references ORDERLINE(id),
	customerID uniqueidentifier not null references CUSTOMER(id),
	trackingNumber int not null,
	arrivalDate date not null,
	isPending boolean not null  
)

create table WORKSWITH (
	distributerID uniqueIdentifier not null references DISTRIBUTER(id),
	shippingCoID uniqueIdentifier not null references SHIPPINGCOMPANY(id)
)

create table HASCARD(
	customerId uniqueidentifier not null references CUSTOMER(id), 
	cardId uniqueidentifier not null references _CARD_(id),
	constraint pk_HASCARD primary key clustered (customerId, cardId)
)

--TODO: CHECKOVER
