# Siemens S7 Windows

A simple application to communicate with the **Siemens S7** protocol.
I use the [S7NetPlus library](https://github.com/S7NetPlus/s7netplus/wiki)

## How to use

Set your PLC changing:
* IP;
* rack;
* slot;
* CPU type.

Once you have done, connect with it. If everythings goes right you'll see the red circle turn in green and a succeful message at the bottom.
Now that you are connected to the PLC you can read or write to the nodes (**NB** In *PLC Settings* there is a checkbox that allow you to change the task, if you need to read, uncheck it, otherwise check it).

If you need to read, there are more fields to fill, for example your node is DB1000.DBB140:

* Data type: select *Data Block*;
* DB: 1000;
* Start byte: 140;
* Var type: in this case, String;
* How many byte: 40, the size of my string;

If you need to write, you just enter the node name and its value (for example: node => DB1000.DBB140; value => "Hello world").

****
