

## Excersice

Please write a console application with the following behaviour.
 

1. When the user enters the name of a shape followed by the corresponding number of numeric parameters, define that shape and keep it in memory.  The numbers may be of type double.  Examples:

    `circle 1.7 ­5.05 6.9`

    `square 3.55 4.1 2.77`

    `rectangle 3.5 2.0 5.6 7.2`

    `triangle 4.5 1 ­2.5 ­33 23 0.3`

    `donut 4.5 7.8 1.5 1.8`


    * For the circle, the numbers are the x and y coordinates of the centre followed by the radius.

    * For the square it is x and y of one corner followed by the length of the side.

    * For the rectangle it is x and y of one corner followed by the two sides.

    * For the triangle it is the x and y coordinates of the three vertices (six numbers in total).

    * For the donut it is the x and y of the centre followed by the two radiuses.

    In addition, every time such a line is entered, the application should give it a unique identifier and print it out in a standardised form, for example:

    => shape 1: circle with centre at (1.7, ­5.05) and radius 6.9

2. When the user enters a pair of numbers, the application should print out all the shapes that include that point in the (x, y) space, i.e. it should print out shape X if the given point is inside X.  (A point is inside a donut shape if it is inside the outer circle but not inside the inner one.)

    It should also print out the surface area of each shape found, and the total area of all the shapes returned for a given point.

3. It should accept the commands “help” for printing instructions and “exit” for terminating the execution.

4. If the user enters anything unexpected (including errors like too few/many arguments, incorrect number format, etc.), it should print a meaningful error message and continue the execution.

5. Feel free to add additional shapes (e.g. ellipsis) or operations (e.g. to delete a given shape).  An advanced option could be to find all the shapes that overlap one that’s named by the user.

6. Allow input from a file as well as the console.  It should be possible, for example, to read a file with shape definitions and then to continue with an interactive session.

7. Think about implementing it in a way which would perform well even for a very large number shapes (e.g., tens of millions, but assuming it can still be held in the program memory).

8. Please provide a sample input file for testing.

# Implementation

For this solution 2 applications, 10 commands and 9 shapes were implemented. The first application: Lucas is the console application that was required for the exercise. The second Lucas.WPF is a WPF application where the shapes are optionally visualized.

## Sample input file

The solution contains a sample input file. With the intention of making the testing process the inputs in the sample file starts with the key with which each element is to be stored in the memory repository. The same semantic can be used so replace an existing element by starting the statement with the key for that shape.

## Building the solution

In the Lucas project there is a post build statement that copies the Universe.txt file to the target directory.

Even the execution of that statemented is successful as intended, it shows the following error:

`Error	1	The command "xcopy "C:\Code\RF-Test\RF-Test\src\Lucas\Universe.txt" "C:\Code\RF-Test\RF-Test\src\Lucas\bin\Debug\" /Y /E /D" exited with code 4.	Lucas`

Please ignore that error and continue to run "the last successful build".

## Usage

1. Open the console application and optionally open the WPF application, if both applications are opened it will produce a TCP handshake and so the WPF application will be listening to some of the user inputs at the console.

2. Type `load universe.txt` and hit enter.

    See that the shapes loaded are listed on the console. An if you had the WPF window opened, you would see something like this:

    ![alt text](https://github.com/lucasoromi/RF-Test/raw/master/images/wpf-screenshot.png "WPF application screenshot")

    A picture is worth a thousand words.

3. Moving the mouse over the WPF application provides feedback about the position and the shapes under the cursor.

# Help

## Commands

### count
###### Syntax: `count [shape1, ... shapeN]`
###### Summary:
Returns the number of existing shape elements. If one or more shape types are specifies as arguments the result will be filtered. If no argument is specified all it will be the total of all.
###### Example: `count circle ellipse`

### clear
###### Syntax: `clear`
###### Summary:
Clear the screen
###### Example: `clear`

### delete
###### Syntax: `delete all | [key1, ...keyn]`
###### Summary:
Deletes a shape given a key.
###### Example: `delete all` `delete 5`

### draw
###### Syntax: `draw auto | draw off | draw [shape1, ... shapeN]`
###### Summary:
Draws a given shape (by key) on the the existing WPF window. If the word auto is specified instead of the shape key, every time a shape is listed it will get automatically drawn in at the target. It will also print the definition of the shape in the console with a friendly format.
###### Example: `draw auto`

### exit
###### Syntax: `exit`
###### Summary:
Exit and close the application
###### Example: `exit`

### help
###### Syntax: `help [shape1, ... shapeN]`
###### Summary:
Lists one or more shape and command definitions. If no argument is specifies it will list all commands and shapes definitions in that order.
###### Example: `help draw circle`

### list
###### Syntax: `list [shape1, ... shapeN]`
###### Summary:
List a set of shapes. If one or more shape types are specifies as arguments the result will be filtered. If no argument is specified all existing shapes will be enumerated.
###### Example: `list circle ellipse`

### load
###### Syntax: `load filename1 [filename2, ... filenameN]`
###### Summary:
Loads shape definitions from one or multiple files at once.
###### Example: `load universe.txt`

### over
###### Syntax: `over key`
###### Summary:
Given a shape key, this command lists all the shapes that overlaps the given shape.
###### Example: `over 4`

### save
###### Syntax: `save filename`
###### Summary:
Saves shape definitions to file.
###### Example: `save lucas.txt`

## Shapes

### triangle
###### Summary:
Triangle shape. Three sides.
###### Example: `triangle 4.5 1 -2.5 -33 23 0.3`

### square
###### Summary:
Square shape. Four sides with same length, all internal angles measure 90 degrees.
###### Example: `square 3.55 4.1 2.77`




### circle
###### Summary:
Circle shape.
###### Example: `circle 1.7 -5.05 6.9`

### donut
###### Summary:
Donut shape. Delimited by two concentric circles.
###### Example: `donut 4.5 7.8 1.5 1.8`

### ellipse
###### Summary:
Ellipse shape.
###### Example: `ellipse 1.7 -5.05 6.9 4.6`

### polygon
###### Summary:
A convex polygon. The number of sides is restricted from 3 to 9.
###### Example: `polygon 34 32 45 32 34 65 43 56`

### rectangle
###### Summary:
Rectangle shape. Four sides, all internal angles measure 90 degrees.
###### Example: `rectangle 3.5 2.0 5.6 7.2`

### square
###### Summary:
Square shape. Four sides with same length, all internal angles measure 90 degrees.
###### Example: `square 3.55 4.1 2.77`

### star
###### Syntax: `star center-x center-y edge-x edge-y inner-radio n-edges`
###### Summary:
Polygon with star symmetry. A star is a polygon that has at least 3 outer edges and it is constructed from the center, the first outer edge,  the radio of the internal edge, and the number of outer edges (the total internal edges is the same).
###### Example: `star star 300 378 350 451 19 3`

