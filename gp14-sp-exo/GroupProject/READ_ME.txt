READ ME

SOFTWARE: AUDRI

Specifications:
	1) Unity 5

Build Settings:
	1) Resolution: 1366 x 768
	2) Target Build: Windows
	3) Architecture: x86_64
	4) Ensure when built the Game is in the same file of the initial program
		e.g
			Main Folder pre-Built: C:\Group Project\GroupProject
			Target Folder: C:\Group Project\GroupProject

		This is too avoid the '.arf' not being recognized


Fixes:

	1)To Fix the issue regarding missing library dll's during build time
		  In Unity Go to:
		- Edit -> Project Settings -> Player
		- Under Other settings for PC / Mac change the API Compatibility 
		  Level from ".NET 2.0 Subset" to ".NET 2.0"

