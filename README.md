Built using .NET Framework v4.0

# BUILDING THE APPLICATION
* Using MSBuild
	* Launch the Visual Studio developer command prompt
	* Navigate to the directory containing the .sln file for the project
	* Run `MSBuild`
* Using Visual Studio
	* Open the .sln file in Visual Studio
	* Click BUILD > Build Solution

# RUNNING THE APPLICATION
* Using the command line
	* Navigate to the ViagogoApp/bin directory
	* Launch the ViagogoApp.exe binary located in the directory corresponding to the build configuration used (Debug/Release)
* Using Visual Studio
	* Open the .sln file in Visual Studio
	* Select a build configuration (Release/Debug) and click Start

# ASSUMPTIONS
* All coordinates in the world have an event
	* The requirements specify that each coordinate should have a maximum of 1 event, but do not specify the minimum number of events
	* The implementation facilitates setting the chance of an event at a given coordinate, but defaults to 100%
* An event can have a maximum of 5 tickets
	* This assumption was made in order to provide a reasonable chance of events having no tickets, in order to increase the likelihood of search results including more than just adjacent cells.
* Tickets have a minimum price of $1.00 maximum price of $99.99
	* This assumption was made to maintain the output format for prices of `$__.__`


# ENHANCEMENTS
* Multiple events per location
	* In this scenario, the data model would be extended to provide each Cell object with a non-null collection of 0..N Event objects
	* The World.SearchNearbyEvents method would then collect the events with tickets from each cell, starting with the nearest, until the required number of events had been collected
* Larger world size
	* Incrementally calculate all positions offset by a given distance
		* Determine all coordinate offsets with a Manhattan distance = 1 (i.e. Abs(X) + Abs(Y)), locate those cells and collect the valid events from these cells
		* If the required number of events have not been collected, increase the distance offset by 1, and re-calculate the valid offsets
			* This can be achieved by calculating the positive offsets (e.g. 1,0 and 0,1) then mirroring across the X/Y axis for non-zero values
				* e.g.	(1,0) -> mirror across Y axis -> (-1,0)
				*		(1,1) -> mirror across X & Y & XY axis -> (1,-1) & (-1,1) & (-1,-1)
		* For reference, see larger_world_size.png included in the repository

