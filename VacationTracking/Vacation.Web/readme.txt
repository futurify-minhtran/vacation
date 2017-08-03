Online version is available at following link:
http://withinpixels.com/themes/fuse/documentation/getting-started/installation

--------------------------------------------------------

A. Installing Prerequisites
Download and install the latest Node.js from its web site.
Download and install the latest Git from its web site.
Open your favorite console application (Terminal, Command Prompt etc.), run the following command and wait for it to finish:
npm install -g bower
Run the following command and wait for it to finish:
npm install -g gulp
Now you are ready to install the Fuse.

B. Installing Project
Navigate to Dashboard project root folder, open a command line and run:
npm install
This command will install all the required Node.js modules into the node_modules directory inside your work folder.

Note: Installing Node.js packages may throw a lot of warnings and errors along the way. As long as the process finishes without any error notes such as "Killed", you will be fine.

Run the following command and wait it for to finish:
bower install
This command will install all the required bower packages into the bower_components directory inside your work folder.

-------------------------------------------------------
IMPORTANT
Before deploy dashboard to server, run following command
gulp build

