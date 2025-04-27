# ciat
Cesar Ivorra Automation Tool

## Dependencies
- dotnet
- dotnet-script (generate csprojs)
- visual studio (debug commands)

### Prerequisites
- **.NET SDK**:
  - Ensure you have the **.NET SDK** (version 9.0 or higher) installed.
  - You can download it from [dotnet.microsoft.com](https://dotnet.microsoft.com/download).
- **.NET CLI**:
  - Verify that the .NET CLI (`dotnet`) is available on your system by running:
    ```powershell
    dotnet --version
    ```
- **dotnet-script**
  - Install dotnet-script as global tool.
    ```powershell
    dotnet tool install -g dotnet-script
    ```
  - Verify that the `dotnet-script` is available on your system by running:
    ```powershell
    dotnet-script --version
    ```

## Compile
TODO

## Comand to execute a command
- Windows
```powershell
  ciatLauncher.exe <comand_name> --<command_property> <command_property_value> ...
```
- Linux / MacOS
```sh
  ciatLauncher <comand_name> --<command_property> <command_property_value> ...
```

## Examples

### SampleEmpty command
#### execution
```powershell
ciatLauncher.exe Empty
```
#### result
```powershell
Executed class name: 'Empty'
class without properties
```

### SampleProperties command
#### execution
```powershell
ciatLauncher.exe SampleProperties --ByteProperty 255 --SByteProperty -128 --ShortProperty -32768 --UShortProperty 65535 --IntProperty 2147483647 --UIntProperty 4294967295 --LongProperty -9223372036854775808 --ULongProperty 18446744073709551615 --FloatProperty 3.14 --DoubleProperty 2.718 --DecimalProperty 1.618 --BoolProperty true --CharProperty A --StringProperty "Hello world ciat!"
```
#### result
```powershell
2025-04-27 12:01:39 [INFO] [SampleProperties] Executed class name: 'SampleProperties'
2025-04-27 12:01:39 [INFO] [SampleProperties] properties:
2025-04-27 12:01:39 [INFO] [SampleProperties] ByteProperty=255 (System.Byte)
2025-04-27 12:01:39 [INFO] [SampleProperties] SByteProperty=-128 (System.SByte)
2025-04-27 12:01:39 [INFO] [SampleProperties] ShortProperty=-32768 (System.Int16)
2025-04-27 12:01:39 [INFO] [SampleProperties] UShortProperty=65535 (System.UInt16)
2025-04-27 12:01:39 [INFO] [SampleProperties] IntProperty=2147483647 (System.Int32)
2025-04-27 12:01:39 [INFO] [SampleProperties] UIntProperty=4294967295 (System.UInt32)
2025-04-27 12:01:39 [INFO] [SampleProperties] LongProperty=-9223372036854775808 (System.Int64)
2025-04-27 12:01:39 [INFO] [SampleProperties] ULongProperty=18446744073709551615 (System.UInt64)
2025-04-27 12:01:39 [INFO] [SampleProperties] FloatProperty=314 (System.Single)
2025-04-27 12:01:39 [INFO] [SampleProperties] DoubleProperty=2718 (System.Double)
2025-04-27 12:01:39 [INFO] [SampleProperties] DecimalProperty=1618 (System.Decimal)
2025-04-27 12:01:39 [INFO] [SampleProperties] BoolProperty=True (System.Boolean)
2025-04-27 12:01:39 [INFO] [SampleProperties] CharProperty=A (System.Char)
2025-04-27 12:01:39 [INFO] [SampleProperties] StringProperty=Hello world ciat! (System.String)
```

### SampleLogger command
#### execution
```powershell
ciatLauncher.exe SampleLogger
```
#### result
```powershell
2025-04-27 12:06:31 [INFO] [SampleLogger] Executed class name: 'SampleLogger'
2025-04-27 12:06:31 [TRACE] [SampleLogger] This is a trace message
2025-04-27 12:06:31 [DEBUG] [SampleLogger] This is a debug message
2025-04-27 12:06:31 [INFO] [SampleLogger] This is an information message
2025-04-27 12:06:31 [WARN] [SampleLogger] This is a warning message
2025-04-27 12:06:31 [ERROR] [SampleLogger] This is an error message
2025-04-27 12:06:31 [CRIT] [SampleLogger] This is a critical message
```