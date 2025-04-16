--SERVER1 : HÀ NỘI

-- Kết nối đến Huế
EXEC sp_addlinkedserver 
    @server = 'Server_HUE', 
    @srvproduct = '', 
    @provider = 'SQLNCLI', 
    @datasrc = 'HONGTHAM2004\SERVER05';

EXEC sp_addlinkedsrvlogin 
    @rmtsrvname = 'Server_HUE', 
    @useself = 'false', 
    @locallogin = NULL, 
    @rmtuser = 'sa', 
    @rmtpassword = '123456';

-- Kết nối đến Sài Gòn
EXEC sp_addlinkedserver 
    @server = 'Server_SAIGON', 
    @srvproduct = '', 
    @provider = 'SQLNCLI', 
    @datasrc = 'HONGTHAM2004\SERVER06';

EXEC sp_addlinkedsrvlogin 
    @rmtsrvname = 'Server_SAIGON', 
    @useself = 'false', 
    @locallogin = NULL, 
    @rmtuser = 'sa', 
    @rmtpassword = '123456';


-- Tạo linked server tới chính Hà Nội
EXEC sp_addlinkedserver 
    @server = 'Server_HANOI', 
    @srvproduct = '',
    @provider = 'SQLNCLI', 
    @datasrc = 'HONGTHAM2004\SERVER07'; -- hoặc đúng tên server Hà Nội

-- Thêm login
EXEC sp_addlinkedsrvlogin 
    @rmtsrvname = 'Server_HANOI',
    @useself = 'false',
    @locallogin = NULL,
    @rmtuser = 'sa',
    @rmtpassword = '123456';





-- Xóa login
EXEC sp_droplinkedsrvlogin @rmtsrvname = 'Server_HANOI', @locallogin = NULL;
EXEC sp_droplinkedsrvlogin @rmtsrvname = 'Server_SAIGON', @locallogin = NULL;
EXEC sp_droplinkedsrvlogin @rmtsrvname = 'Server_HUE', @locallogin = NULL;

-- Xóa linked server
EXEC sp_dropserver @server = 'Server_HANOI', @droplogins = 'droplogins';
EXEC sp_dropserver @server = 'Server_SAIGON', @droplogins = 'droplogins';
EXEC sp_dropserver @server = 'Server_HUE', @droplogins = 'droplogins';