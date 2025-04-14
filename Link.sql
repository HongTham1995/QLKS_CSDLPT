--SERVER1 : HÀ NỘI

-- Tạo Linked Server cho Huế từ Hà Nội
EXEC sp_addlinkedserver 
    @server = 'Server_HUE', 
    @srvproduct = '',
    @provider = 'SQLNCLI',
    @datasrc = 'HONGTHAM2004\SERVER05';

-- Tạo Linked Server cho Sài Gòn từ Hà Nội
EXEC sp_addlinkedserver 
    @server = 'Server_SAIGON', 
    @srvproduct = '',
    @provider = 'SQLNCLI',
    @datasrc = 'HONGTHAM2004\SERVER06';

-- Cho phép đăng nhập vào Huế
EXEC sp_addlinkedsrvlogin 
    @rmtsrvname = 'Server_HUE',
    @useself = 'true';

-- Cho phép đăng nhập vào Sài Gòn
EXEC sp_addlinkedsrvlogin 
    @rmtsrvname = 'Server_SAIGON',
    @useself = 'true';

--SERVER2: HUẾ
-- Tạo Linked Server cho Hà Nội từ Huế
EXEC sp_addlinkedserver 
    @server = 'Server_HANOI', 
    @srvproduct = '',
    @provider = 'SQLNCLI',
    @datasrc = 'HONGTHAM2004\SERVER04';

-- Tạo Linked Server cho Sài Gòn từ Huế
EXEC sp_addlinkedserver 
    @server = 'Server_SAIGON', 
    @srvproduct = '',
    @provider = 'SQLNCLI',
    @datasrc = 'HONGTHAM2004\SERVER06';

EXEC sp_addlinkedsrvlogin 
    @rmtsrvname = 'Server_HANOI',
    @useself = 'true';


EXEC sp_addlinkedsrvlogin 
    @rmtsrvname = 'Server_SAIGON',
    @useself = 'true';

--SERVER3: TPHCM
-- Tạo Linked Server cho Hà Nội từ Sài Gòn
EXEC sp_addlinkedserver 
    @server = 'Server_HANOI', 
    @srvproduct = '',
    @provider = 'SQLNCLI',
    @datasrc = 'HONGTHAM2004\SERVER04';

-- Tạo Linked Server cho Huế từ Sài Gòn
EXEC sp_addlinkedserver 
    @server = 'Server_HUE', 
    @srvproduct = '',
    @provider = 'SQLNCLI',
    @datasrc = 'HONGTHAM2004\SERVER05';

-- Cho phép đăng nhập vào Hà Nội
EXEC sp_addlinkedsrvlogin 
    @rmtsrvname = 'Server_HANOI',
    @useself = 'true';

-- Cho phép đăng nhập vào Huế
EXEC sp_addlinkedsrvlogin 
    @rmtsrvname = 'Server_HUE',
    @useself = 'true';

