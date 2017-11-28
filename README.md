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
* The chance of an event existing in a given coordinate is 50%
* An event can have a maximum of 5 tickets
* Tickets have a minimum price of $1.00 maximum price of $99.99


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

