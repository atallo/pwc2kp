# PWC2KP - Password Corral to KeePass

Converts a password store of Password Corral to one of KeePass/Bitwarden (or in general almost any password manager)

Important, unlike other applications available, **folders are migrated**.

This program is not very pretentious, I made it, used it and published it in case someone else has the same need. If you are interested in adding any new functionality, let me know and I will gladly give you its development.

Please don't make unnecessary forks.

More information:
* Password Corral http://www.cygnusproductions.com/freeware/pc.asp
* Keepass https://keepass.info/

# Security

This program is 100% virus free. Currently some bad antivirus detect it as a hacking tool or a Trojan. That is because 
this program to obtain the data uses a similar technique to some of those programs. If you are not comfortable with this problem, 
I recommend that you use the sources directly, or run the program in a virtual machine.

https://www.virustotal.com/gui/url/e06156828046fadaef225ba4fe2043b9f454589330f2f47ad3e8af7647a3b8a8/detection

## Use:
1. Run the app
2. Run a Password Corral
3. Login to it.
4. The current version of this program does not migrate the latest group. You have to create a last group that is junk.
5. Write where the data will be exported
6. Press the "Export" button
7. Wait
8. Import the csv file generated in Keepass

Note: I used the program to migrate my passwords, it works but be careful, make a backup first.

The only functionality that is not migrated is the time of the passwords. I don't use it and it never seemed important to me, if someone is interested in migrating that functionality, the modification is simple.

In Password Corral you can add a mail field that does not exist in Keepass. This will be migrated in the "username" field or in "notes".

## Notes:
* I use a version of ManagedWinapi DLL. An unmodified version could be used but I wanted to finish this program soon.
* You can also use other programs like keeweb, keepassXC...

## Donwload
* https://github.com/atallo/pwc2kp/releases

## Requires
* Windows
* .NET Framework 4.8
