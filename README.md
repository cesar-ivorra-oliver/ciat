# ciat
Cesar Ivorra Automation Tool

## Comand to execute a command
```sh
  ciatLauncher.exe <comand_name> --<command_property> <command_property_value> ...
```

### Execution of Sample command
```sh
ciatLauncher.exe Sample --ByteProperty 255 --SByteProperty -128 --ShortProperty -32768 --UShortProperty 65535 --IntProperty 2147483647 --UIntProperty 4294967295 --LongProperty -9223372036854775808 --ULongProperty 18446744073709551615 --FloatProperty 3.14 --DoubleProperty 2.718 --DecimalProperty 1.618 --BoolProperty true --CharProperty A --StringProperty "Hello world ciat!"
```

### Example result
```sh
Executed class name: DataTypeDemo;
params:
ByteProperty=255 (System.Byte),
SByteProperty=-128 (System.SByte),
ShortProperty=-32768 (System.Int16),
UShortProperty=65535 (System.UInt16),
IntProperty=2147483647 (System.Int32),
UIntProperty=4294967295 (System.UInt32),
LongProperty=-9223372036854775808 (System.Int64),
ULongProperty=18446744073709551615 (System.UInt64),
FloatProperty=3.14 (System.Single),
DoubleProperty=2.718 (System.Double),
DecimalProperty=1.618 (System.Decimal),
BoolProperty=True (System.Boolean),
CharProperty=A (System.Char).
```
