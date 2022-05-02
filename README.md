# AutoBot
A spotify (and more) stream bot proof of concept.

# Please don't judge my coding skills based on this project :-D
I did this a while ago as a proof of concept. A lot of parts could look like I had a seizure, since it wasn't meant to be a serious product or something like that, just shitting ideas to the display.

# Why did I do it
I often heared about those skids pretending to bot spotify clicks on german documentaries. On the first look I could say what they do is bullshit. I wanted to check if spotify has any counter measures against stream botting. A little hint. They have, but they are far from good.

# On what is the project based
Webserver is written in PHP to authenticate the clients, management console and provide additional data. 
The data is stored in a mysql database, the clients and management console establish direct connections to the provided database.
Client and management console is written in C#. Selenium is used for webautomation, because the first thought I had writing this is, emulating web requests is to time consuming. Obviously emulating web requests would be a lot better, but hey..

# Where should the clients run on?
Best is to run it in vms. For tests I used a Hypervisor (VMWare Esxi) and created Windows Server 2012 r2 VMs. Windows Server 2012 was the best choice, since the non UI version consumes no ressources at all. I tested with around 130 VMs, each with 512 MB ram and one cpu core. To prevent ram bottlenecking I extended the swap disk to 4096 MB.
**Specs my test system had**
Processor: Ryzen 3900x
RAM: 128 GB
Disk: 4 TB NVME SSD

# What about my research results?
As a good researcher I lost the paper I wrote. Its pretty simple. When I tested the automation (2020), spotify did not had any seriously good mechanisms to detect the automation. Even removing obvious selenium flags and patching the binaries wasn't needed. They have a lot of statistics based checks. I was able to beat their system by creating hundreds of artists using automations (not included in the project) and providing automatically generated songs. Each song and artist just had a little streams each month, but in total they did around nine million streams per month. Meanwhile spotify started to block VPN providers. The openvpn integration allowed to automatically connect to perfect privacys vpn network. Now you would need to setup a own vpn network or use socks proxys.

# Project support
I do not help you to setup the bot. It is for research only and if youre writing your own one, or doing researches, this project might help you a little.
Please remember that faking streams and gaining revenue is not legal. @Spotify, please don't sue me for this research. :-D
