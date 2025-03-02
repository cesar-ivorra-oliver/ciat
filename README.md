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

### Examples
#### Execution of Sample command
```powershell
ciatLauncher.exe Sample --ByteProperty 255 --SByteProperty -128 --ShortProperty -32768 --UShortProperty 65535 --IntProperty 2147483647 --UIntProperty 4294967295 --LongProperty -9223372036854775808 --ULongProperty 18446744073709551615 --FloatProperty 3.14 --DoubleProperty 2.718 --DecimalProperty 1.618 --BoolProperty true --CharProperty A --StringProperty "Hello world ciat!"
```

#### Sample command result
```powershell
Executed class name: 'Sample'
properties:
ByteProperty=255 (System.Byte)
SByteProperty=-128 (System.SByte)
ShortProperty=-32768 (System.Int16)
UShortProperty=65535 (System.UInt16)
IntProperty=2147483647 (System.Int32)
UIntProperty=4294967295 (System.UInt32)
LongProperty=-9223372036854775808 (System.Int64)
ULongProperty=18446744073709551615 (System.UInt64)
FloatProperty=314 (System.Single)
DoubleProperty=2718 (System.Double)
DecimalProperty=1618 (System.Decimal)
BoolProperty=True (System.Boolean)
CharProperty=A (System.Char)
StringProperty=Hello world ciat! (System.String)
```

#### Execution of Empty command
```powershell
ciatLauncher.exe Empty
```

#### Empty command result
```powershell
Executed class name: 'Empty'
class without properties
```