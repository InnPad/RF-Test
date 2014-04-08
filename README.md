

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

4. Type `clear` and hit enter.

    All your shapes will remain on the console repository, but the console screen will be cleared, and if you had the WPF application opened, it will be cleared as well.

5. Enable sending all shapes resulting from a query to the WPF canvas at once

    Type `draw auto` and hit enter.

6. Now query all the shapes intersecting the donut (key 37)

    Type `over 37` and hit enter.

    Now you should be getting all these shapes listed

    `=> Listing Shapes overlaping 37 donut with centre at (410, 431), internal radius 78 and external radius 137`
    `=> shape 17: polygon with vertexes (350, 350), (310.72, 339), (300, 365), (289.28, 339), (263, 350), (274.13, 324), (247.67, 313), (274.13, 302), (263, 276), (289.28, 287), (300, 261), (310.72, 287), (337, 276), (325.87, 302), (352.33, 313, (325.87, 324)`
    `=> shape 18: polygon with vertexes (450, 350), (403.13, 344), (383.31, 357), (382.18, 333), (363.68, 318), (385.86, 310), (394.25, 288), (409.08, 306), (432.76, 307), (419.75, 327)`
    `=> shape 19: polygon with vertexes (560, 330), (504.65, 303), (504.56, 345), (497.15, 304), (469.99, 336), (490.99, 299), (449.46, 307), (489.04, 292), (452.58, 271), (492.22, 285), (477.89, 246), (499.04, 282), (513.54, 242), (506.31, 284), (542.86, 263), (510.63, 290), (552.13, 298), (509.97, 298)`
    `=> shape 23: polygon with vertexes (250, 462), (208.59, 381), (183.22, 505), (186.17, 378), (73.07, 436), (174.16, 359), (58.51, 306), (181.62, 338), (150.49, 215), (202.91, 330), (279.76, 229), (222.01, 342), (348.96, 339), (224.54, 365)`
    `=> shape 24: polygon with vertexes (350, 451), (295.08, 396), (200.28, 405), (286.56, 365), (326.72, 278), (318.35, 373)`
    `=> shape 26: polygon with vertexes (560, 467), (503.29, 433), (464.69, 481), (481.29, 422), (423.18, 400), (485.15, 397), (487.83, 335), (509.53, 393), (569.3,377), (520.74, 415)`
    `=> shape 27: polygon with vertexes (660, 450), (613.42, 410), (619.54, 475), (598, 413), (567.88, 471), (583.22, 407), (526.41, 440), (573.77, 395), (508.31, 392), (572.64, 379), (519.32, 341), (580.2, 365), (555.95, 304), (594.05, 358), (606.56, 293), (609.78, 359), (655.09, 311), (622.42, 368), (686.13, 353), (627.93, 383), (689.82, 405), (624.58, 398)`
    `=> shape 31: polygon with vertexes (350, 550), (311.41, 535), (304.81, 566), (293.01, 537), (268.36, 556), (277.88, 526), (246.73, 525), (273.1, 509), (250.01, 488), (280.91, 492), (276.69, 461), (297.65, 484), (314.27, 458), (315.49, 489), (345.18, 479), (326.08, 504), (354.94, 516), (324.47, 522)`
    `=> shape 32: polygon with vertexes (450, 550), (404.14, 493), (373.65, 576), (388.69, 489), (301.65, 504), (384.55, 474), (328, 406), (395.86, 463), (426.35, 380), (411.31, 467), (498.35, 452), (415.45, 482)`
    `=> shape 33: polygon with vertexes (560, 550), (500, 522), (449, 550), (477, 499), (449, 448), (500, 476), (551, 448), (523, 499)`
    `=> shape 39: circle with centre at (432, 331) and radius 60`
    `=> shape 40: rectangle with top left cornet at (540, 431), height 84 and width 84`

    And if you had the WPF canvas open, those shapes are displayed as well

    ![alt text](https://github.com/lucasoromi/RF-Test/raw/master/images/wpf-screenshot-query.png "WPF application screenshot")

7. Now lets display the donut itself

    Type `draw 37` and hit enter.

    ![alt text](https://github.com/lucasoromi/RF-Test/raw/master/images/wpf-screenshot-overlay.png "WPF application screenshot")

    Now you have the full picture!


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

