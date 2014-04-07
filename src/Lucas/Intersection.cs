using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*****
*
*   Intersection.js
*
*   copyright 2002, Kevin Lindsey
*
*****/

namespace Lucas
{
    using Lucas.Shapes;

    /// <summary>
    /// Taken from http://www.kevlindev.com/gui/math/intersection/Intersection.js
    /// </summary>
    public class Intersection
    {
        object status;
        List<Point> points;

        public Intersection(object status)
        {
            this.init(status);
        }

        void init(object status) {
    this.status = status;
    this.points = new List<Point>();
}


/*****
*
*   appendPoint
*
*****/
 void appendPoint(Point point) {
    this.points.Add(point);
}


/*****
*
*   appendPoints
*
*****/
void appendPoints(params Point[] points) {
    this.points.AddRange(points);
}


/*****
*
*   class methods
*
*****/

/*****
*
*   intersectShapes
*
*****/
public bool intersectShapes(IShape shape1, IShape shape2) {
    var ip1 = shape1.getIntersectionParams();
    var ip2 = shape2.getIntersectionParams();
    var result;

    if ( ip1 != null && ip2 != null ) {
        if ( ip1.name == "Path" ) {
            result = Intersection.intersectPathShape(shape1, shape2);
        } else if ( ip2.name == "Path" ) {
            result = Intersection.intersectPathShape(shape2, shape1);
        } else {
            var method;
            var params;

            if ( ip1.name < ip2.name ) {
                method = "intersect" + ip1.name + ip2.name;
                params = ip1.params.concat( ip2.params );
            } else {
                method = "intersect" + ip2.name + ip1.name;
                params = ip2.params.concat( ip1.params );
            }

            if ( !(method in Intersection) )
                throw new Error("Intersection not available: " + method);

            result = Intersection[method].apply(null, params);
        }
    } else {
        result = new Intersection("No Intersection");
    }

    return result;
}


/*****
*
*   intersectPathShape
*
*****/
object intersectPathShape(Point[] path, IShape shape) {
    return path.intersectShape(shape);
}


/*****
*
*   intersectBezier2Bezier2
*
*****/
bool intersectBezier2Bezier2(double a1,double a2,double a3,double b1,double b2,double b3) {
    var a, b;
    var c12, c11, c10;
    var c22, c21, c20;
    var result = new Intersection("No Intersection");
    var poly;

    a = a2.multiply(-2);
    c12 = a1.add(a.add(a3));

    a = a1.multiply(-2);
    b = a2.multiply(2);
    c11 = a.add(b);

    c10 = new Point2D(a1.X, a1.Y);

    a = b2.multiply(-2);
    c22 = b1.add(a.add(b3));

    a = b1.multiply(-2);
    b = b2.multiply(2);
    c21 = a.add(b);

    c20 = new Point2D(b1.X, b1.Y);
    
    if ( c12.Y == 0 ) {
        var v0 = c12.X*(c10.Y - c20.Y);
        var v1 = v0 - c11.X*c11.Y;
        var v2 = v0 + v1;
        var v3 = c11.Y*c11.Y;

        poly = new Polynomial(
            c12.X*c22.Y*c22.Y,
            2*c12.X*c21.Y*c22.Y,
            c12.X*c21.Y*c21.Y - c22.X*v3 - c22.Y*v0 - c22.Y*v1,
            -c21.X*v3 - c21.Y*v0 - c21.Y*v1,
            (c10.X - c20.X)*v3 + (c10.Y - c20.Y)*v1
        );
    } else {
        var v0 = c12.X*c22.Y - c12.Y*c22.X;
        var v1 = c12.X*c21.Y - c21.X*c12.Y;
        var v2 = c11.X*c12.Y - c11.Y*c12.X;
        var v3 = c10.Y - c20.Y;
        var v4 = c12.Y*(c10.X - c20.X) - c12.X*v3;
        var v5 = -c11.Y*v2 + c12.Y*v4;
        var v6 = v2*v2;

        poly = new Polynomial(
            v0*v0,
            2*v0*v1,
            (-c22.Y*v6 + c12.Y*v1*v1 + c12.Y*v0*v4 + v0*v5) / c12.Y,
            (-c21.Y*v6 + c12.Y*v1*v4 + v1*v5) / c12.Y,
            (v3*v6 + v4*v5) / c12.Y
        );
    }

    var roots = poly.getRoots();
    for ( var i = 0; i < roots.Length; i++ ) {
        var s = roots[i];

        if ( 0 <= s && s <= 1 ) {
            var xRoots = new Polynomial(
                c12.X,
                c11.X,
                c10.X - c20.X - s*c21.X - s*s*c22.X
            ).getRoots();
            var yRoots = new Polynomial(
                c12.Y,
                c11.Y,
                c10.Y - c20.Y - s*c21.Y - s*s*c22.Y
            ).getRoots();

            if ( xRoots.Length > 0 && yRoots.Length > 0 ) {
                var TOLERANCE = 1e-4;

                checkRoots:
                for ( var j = 0; j < xRoots.Length; j++ ) {
                    var xRoot = xRoots[j];

                    if ( 0 <= xRoot && xRoot <= 1 ) {
                        for ( var k = 0; k < yRoots.Length; k++ ) {
                            if ( Math.abs( xRoot - yRoots[k] ) < TOLERANCE ) {
                                result.points.push( c22.multiply(s*s).add(c21.multiply(s).add(c20)) );
                                break checkRoots;
                            }
                        }
                    }
                }
            }
        }
    }

    if ( result.points.Length > 0 ) result.status = "Intersection";

    return result;
}


/*****
*
*   intersectBezier2Bezier3
*
*****/
public static Intersection intersectBezier2Bezier3(Point a1, Point a2, Point a3, Point b1, Point b2, Point b3, Point b4) {
    Point a, b,c, d;
    Point c12, c11, c10;
    Point c23, c22, c21, c20;
    var result = new Intersection("No Intersection");

    a = a2.multiply(-2);
    c12 = a1.add(a.add(a3));

    a = a1.multiply(-2);
    b = a2.multiply(2);
    c11 = a.add(b);

    c10 = new Point { X = a1.X, Y = a1.Y };

    a = b1.multiply(-1);
    b = b2.multiply(3);
    c = b3.multiply(-3);
    d = a.add(b.add(c.add(b4)));
    c23 = new Vector2D(d.X, d.Y);

    a = b1.multiply(3);
    b = b2.multiply(-6);
    c = b3.multiply(3);
    d = a.add(b.add(c));
    c22 = new Vector2D(d.X, d.Y);

    a = b1.multiply(-3);
    b = b2.multiply(3);
    c = a.add(b);
    c21 = new Vector2D(c.X, c.Y);

    c20 = new Vector2D(b1.X, b1.Y);

    var c10x2 = c10.X*c10.X;
    var c10y2 = c10.Y*c10.Y;
    var c11x2 = c11.X*c11.X;
    var c11y2 = c11.Y*c11.Y;
    var c12x2 = c12.X*c12.X;
    var c12y2 = c12.Y*c12.Y;
    var c20x2 = c20.X*c20.X;
    var c20y2 = c20.Y*c20.Y;
    var c21x2 = c21.X*c21.X;
    var c21y2 = c21.Y*c21.Y;
    var c22x2 = c22.X*c22.X;
    var c22y2 = c22.Y*c22.Y;
    var c23x2 = c23.X*c23.X;
    var c23y2 = c23.Y*c23.Y;

    var poly = new Polynomial(
        -2*c12.X*c12.Y*c23.X*c23.Y + c12x2*c23y2 + c12y2*c23x2,
        -2*c12.X*c12.Y*c22.X*c23.Y - 2*c12.X*c12.Y*c22.Y*c23.X + 2*c12y2*c22.X*c23.X +
            2*c12x2*c22.Y*c23.Y,
        -2*c12.X*c21.X*c12.Y*c23.Y - 2*c12.X*c12.Y*c21.Y*c23.X - 2*c12.X*c12.Y*c22.X*c22.Y +
            2*c21.X*c12y2*c23.X + c12y2*c22x2 + c12x2*(2*c21.Y*c23.Y + c22y2),
        2*c10.X*c12.X*c12.Y*c23.Y + 2*c10.Y*c12.X*c12.Y*c23.X + c11.X*c11.Y*c12.X*c23.Y +
            c11.X*c11.Y*c12.Y*c23.X - 2*c20.X*c12.X*c12.Y*c23.Y - 2*c12.X*c20.Y*c12.Y*c23.X -
            2*c12.X*c21.X*c12.Y*c22.Y - 2*c12.X*c12.Y*c21.Y*c22.X - 2*c10.X*c12y2*c23.X -
            2*c10.Y*c12x2*c23.Y + 2*c20.X*c12y2*c23.X + 2*c21.X*c12y2*c22.X -
            c11y2*c12.X*c23.X - c11x2*c12.Y*c23.Y + c12x2*(2*c20.Y*c23.Y + 2*c21.Y*c22.Y),
        2*c10.X*c12.X*c12.Y*c22.Y + 2*c10.Y*c12.X*c12.Y*c22.X + c11.X*c11.Y*c12.X*c22.Y +
            c11.X*c11.Y*c12.Y*c22.X - 2*c20.X*c12.X*c12.Y*c22.Y - 2*c12.X*c20.Y*c12.Y*c22.X -
            2*c12.X*c21.X*c12.Y*c21.Y - 2*c10.X*c12y2*c22.X - 2*c10.Y*c12x2*c22.Y +
            2*c20.X*c12y2*c22.X - c11y2*c12.X*c22.X - c11x2*c12.Y*c22.Y + c21x2*c12y2 +
            c12x2*(2*c20.Y*c22.Y + c21y2),
        2*c10.X*c12.X*c12.Y*c21.Y + 2*c10.Y*c12.X*c21.X*c12.Y + c11.X*c11.Y*c12.X*c21.Y +
            c11.X*c11.Y*c21.X*c12.Y - 2*c20.X*c12.X*c12.Y*c21.Y - 2*c12.X*c20.Y*c21.X*c12.Y -
            2*c10.X*c21.X*c12y2 - 2*c10.Y*c12x2*c21.Y + 2*c20.X*c21.X*c12y2 -
            c11y2*c12.X*c21.X - c11x2*c12.Y*c21.Y + 2*c12x2*c20.Y*c21.Y,
        -2*c10.X*c10.Y*c12.X*c12.Y - c10.X*c11.X*c11.Y*c12.Y - c10.Y*c11.X*c11.Y*c12.X +
            2*c10.X*c12.X*c20.Y*c12.Y + 2*c10.Y*c20.X*c12.X*c12.Y + c11.X*c20.X*c11.Y*c12.Y +
            c11.X*c11.Y*c12.X*c20.Y - 2*c20.X*c12.X*c20.Y*c12.Y - 2*c10.X*c20.X*c12y2 +
            c10.X*c11y2*c12.X + c10.Y*c11x2*c12.Y - 2*c10.Y*c12x2*c20.Y -
            c20.X*c11y2*c12.X - c11x2*c20.Y*c12.Y + c10x2*c12y2 + c10y2*c12x2 +
            c20x2*c12y2 + c12x2*c20y2
    );
    var roots = poly.getRootsInInterval(0,1);

    for ( var i = 0; i < roots.Length; i++ ) {
        var s = roots[i];
        var xRoots = new Polynomial(
            c12.X,
            c11.X,
            c10.X - c20.X - s*c21.X - s*s*c22.X - s*s*s*c23.X
        ).getRoots();
        var yRoots = new Polynomial(
            c12.Y,
            c11.Y,
            c10.Y - c20.Y - s*c21.Y - s*s*c22.Y - s*s*s*c23.Y
        ).getRoots();

        if ( xRoots.Length > 0 && yRoots.Length > 0 ) {
            var TOLERANCE = 1e-4;

            checkRoots:
            for ( var j = 0; j < xRoots.Length; j++ ) {
                var xRoot = xRoots[j];
                
                if ( 0 <= xRoot && xRoot <= 1 ) {
                    for ( var k = 0; k < yRoots.Length; k++ ) {
                        if ( Math.abs( xRoot - yRoots[k] ) < TOLERANCE ) {
                            result.points.push(
                                c23.multiply(s*s*s).add(c22.multiply(s*s).add(c21.multiply(s).add(c20)))
                            );
                            break checkRoots;
                        }
                    }
                }
            }
        }
    }

    if ( result.points.Length > 0 ) result.status = "Intersection";

    return result;

}


/*****
*
*   intersectBezier2Circle
*
*****/
public static Intersection intersectBezier2Circle(Point p1, Point p2, Point p3, Point c, double r) {
    return Intersection.intersectBezier2Ellipse(p1, p2, p3, c, r, r);
}


/*****
*
*   intersectBezier2Ellipse
*
*****/
public static Intersection intersectBezier2Ellipse(Point p1, Point p2, Point p3, Point ec, double rx, double ry) {
    Point a, b;       // temporary variables
    Point c2, c1, c0; // coefficients of quadratic
    var result = new Intersection("No Intersection");

    a = p2.multiply(-2);
    c2 = p1.add(a.add(p3));

    a = p1.multiply(-2);
    b = p2.multiply(2);
    c1 = a.add(b);

    c0 = new Point2D(p1.X, p1.Y);

    var rxrx  = rx*rx;
    var ryry  = ry*ry;
    var roots = new Polynomial(
        ryry*c2.X*c2.X + rxrx*c2.Y*c2.Y,
        2*(ryry*c2.X*c1.X + rxrx*c2.Y*c1.Y),
        ryry*(2*c2.X*c0.X + c1.X*c1.X) + rxrx*(2*c2.Y*c0.Y+c1.Y*c1.Y) -
            2*(ryry*ec.X*c2.X + rxrx*ec.Y*c2.Y),
        2*(ryry*c1.X*(c0.X-ec.X) + rxrx*c1.Y*(c0.Y-ec.Y)),
        ryry*(c0.X*c0.X+ec.X*ec.X) + rxrx*(c0.Y*c0.Y + ec.Y*ec.Y) -
            2*(ryry*ec.X*c0.X + rxrx*ec.Y*c0.Y) - rxrx*ryry
    ).getRoots();

    for ( var i = 0; i < roots.Length; i++ ) {
        var t = roots[i];

        if ( 0 <= t && t <= 1 )
            result.points.push( c2.multiply(t*t).add(c1.multiply(t).add(c0)) );
    }

    if ( result.points.Length > 0 ) result.status = "Intersection";

    return result;
}


/*****
*
*   intersectBezier2Line
*
*****/
public static Intersection intersectBezier2Linefunction(Point p1, Point p2, Point p3, Point a1, Point a2) {
    Point a, b;             // temporary variables
    var c2, c1, c0;       // coefficients of quadratic
    var cl;               // c coefficient for normal form of line
    var n;                // normal for normal form of line
    var min = a1.min(a2); // used to determine if point is on line segment
    var max = a1.max(a2); // used to determine if point is on line segment
    var result = new Intersection("No Intersection");
    
    a = p2.multiply(-2);
    c2 = p1.add(a.add(p3));

    a = p1.multiply(-2);
    b = p2.multiply(2);
    c1 = a.add(b);

    c0 = new Point2D(p1.X, p1.Y);

    // Convert line to normal form: ax + by + c = 0
    // Find normal to line: negative inverse of original line's slope
    n = new Vector2D(a1.Y - a2.Y, a2.X - a1.X);
    
    // Determine new c coefficient
    cl = a1.X*a2.Y - a2.X*a1.Y;

    // Transform cubic coefficients to line's coordinate system and find roots
    // of cubic
    var roots = new Polynomial(
        n.dot(c2),
        n.dot(c1),
        n.dot(c0) + cl
    ).getRoots();

    // Any roots in closed interval [0,1] are intersections on Bezier, but
    // might not be on the line segment.
    // Find intersections and calculate point coordinates
    for ( var i = 0; i < roots.Length; i++ ) {
        var t = roots[i];

        if ( 0 <= t && t <= 1 ) {
            // We're within the Bezier curve
            // Find point on Bezier
            var p4 = p1.lerp(p2, t);
            var p5 = p2.lerp(p3, t);

            var p6 = p4.lerp(p5, t);

            // See if point is on line segment
            // Had to make special cases for vertical and horizontal lines due
            // to slight errors in calculation of p6
            if ( a1.X == a2.X ) {
                if ( min.Y <= p6.Y && p6.Y <= max.Y ) {
                    result.status = "Intersection";
                    result.appendPoint( p6 );
                }
            } else if ( a1.Y == a2.Y ) {
                if ( min.X <= p6.X && p6.X <= max.X ) {
                    result.status = "Intersection";
                    result.appendPoint( p6 );
                }
            } else if ( p6.gte(min) && p6.lte(max) ) {
                result.status = "Intersection";
                result.appendPoint( p6 );
            }
        }
    }

    return result;
}


/*****
*
*   intersectBezier2Polygon
*
*****/
public static Intersection intersectBezier2Polygon(Point p1, Point p2, Point p3, Point[] points) {
    var result = new Intersection("No Intersection");
    var length = points.Length;

    for ( var i = 0; i < length; i++ ) {
        var a1 = points[i];
        var a2 = points[(i+1) % length];
        var inter = Intersection.intersectBezier2Line(p1, p2, p3, a1, a2);

        result.appendPoints(inter.points);
    }

    if ( result.points.Length > 0 ) result.status = "Intersection";

    return result;
}


/*****
*
*   intersectBezier2Rectangle
*
*****/
public static Intersection intersectBezier2Rectangle(Point p1, Point p2, Point p3, Point r1, Point r2) {
    var min        = r1.min(r2);
    var max        = r1.max(r2);
    var topRight   = new Point2D( max.X, min.Y );
    var bottomLeft = new Point2D( min.X, max.Y );
    
    var inter1 = Intersection.intersectBezier2Line(p1, p2, p3, min, topRight);
    var inter2 = Intersection.intersectBezier2Line(p1, p2, p3, topRight, max);
    var inter3 = Intersection.intersectBezier2Line(p1, p2, p3, max, bottomLeft);
    var inter4 = Intersection.intersectBezier2Line(p1, p2, p3, bottomLeft, min);
    
    var result = new Intersection("No Intersection");

    result.appendPoints(inter1.points);
    result.appendPoints(inter2.points);
    result.appendPoints(inter3.points);
    result.appendPoints(inter4.points);

    if ( result.points.Length > 0 ) result.status = "Intersection";

    return result;
}


/*****
*
*   intersectBezier3Bezier3
*
*****/
public static Intersection intersectBezier3Bezier3(Point a1, Point a2, Point a3, Point a4, Point b1, Point b2, Point b3, Point b4) {
    var a, b, c, d;         // temporary variables
    var c13, c12, c11, c10; // coefficients of cubic
    var c23, c22, c21, c20; // coefficients of cubic
    var result = new Intersection("No Intersection");

    // Calculate the coefficients of cubic polynomial
    a = a1.multiply(-1);
    b = a2.multiply(3);
    c = a3.multiply(-3);
    d = a.add(b.add(c.add(a4)));
    c13 = new Vector2D(d.X, d.Y);

    a = a1.multiply(3);
    b = a2.multiply(-6);
    c = a3.multiply(3);
    d = a.add(b.add(c));
    c12 = new Vector2D(d.X, d.Y);

    a = a1.multiply(-3);
    b = a2.multiply(3);
    c = a.add(b);
    c11 = new Vector2D(c.X, c.Y);

    c10 = new Vector2D(a1.X, a1.Y);

    a = b1.multiply(-1);
    b = b2.multiply(3);
    c = b3.multiply(-3);
    d = a.add(b.add(c.add(b4)));
    c23 = new Vector2D(d.X, d.Y);

    a = b1.multiply(3);
    b = b2.multiply(-6);
    c = b3.multiply(3);
    d = a.add(b.add(c));
    c22 = new Vector2D(d.X, d.Y);

    a = b1.multiply(-3);
    b = b2.multiply(3);
    c = a.add(b);
    c21 = new Vector2D(c.X, c.Y);

    c20 = new Vector2D(b1.X, b1.Y);

    var c10x2 = c10.X*c10.X;
    var c10x3 = c10.X*c10.X*c10.X;
    var c10y2 = c10.Y*c10.Y;
    var c10y3 = c10.Y*c10.Y*c10.Y;
    var c11x2 = c11.X*c11.X;
    var c11x3 = c11.X*c11.X*c11.X;
    var c11y2 = c11.Y*c11.Y;
    var c11y3 = c11.Y*c11.Y*c11.Y;
    var c12x2 = c12.X*c12.X;
    var c12x3 = c12.X*c12.X*c12.X;
    var c12y2 = c12.Y*c12.Y;
    var c12y3 = c12.Y*c12.Y*c12.Y;
    var c13x2 = c13.X*c13.X;
    var c13x3 = c13.X*c13.X*c13.X;
    var c13y2 = c13.Y*c13.Y;
    var c13y3 = c13.Y*c13.Y*c13.Y;
    var c20x2 = c20.X*c20.X;
    var c20x3 = c20.X*c20.X*c20.X;
    var c20y2 = c20.Y*c20.Y;
    var c20y3 = c20.Y*c20.Y*c20.Y;
    var c21x2 = c21.X*c21.X;
    var c21x3 = c21.X*c21.X*c21.X;
    var c21y2 = c21.Y*c21.Y;
    var c22x2 = c22.X*c22.X;
    var c22x3 = c22.X*c22.X*c22.X;
    var c22y2 = c22.Y*c22.Y;
    var c23x2 = c23.X*c23.X;
    var c23x3 = c23.X*c23.X*c23.X;
    var c23y2 = c23.Y*c23.Y;
    var c23y3 = c23.Y*c23.Y*c23.Y;
    var poly = new Polynomial(
        -c13x3*c23y3 + c13y3*c23x3 - 3*c13.X*c13y2*c23x2*c23.Y +
            3*c13x2*c13.Y*c23.X*c23y2,
        -6*c13.X*c22.X*c13y2*c23.X*c23.Y + 6*c13x2*c13.Y*c22.Y*c23.X*c23.Y + 3*c22.X*c13y3*c23x2 -
            3*c13x3*c22.Y*c23y2 - 3*c13.X*c13y2*c22.Y*c23x2 + 3*c13x2*c22.X*c13.Y*c23y2,
        -6*c21.X*c13.X*c13y2*c23.X*c23.Y - 6*c13.X*c22.X*c13y2*c22.Y*c23.X + 6*c13x2*c22.X*c13.Y*c22.Y*c23.Y +
            3*c21.X*c13y3*c23x2 + 3*c22x2*c13y3*c23.X + 3*c21.X*c13x2*c13.Y*c23y2 - 3*c13.X*c21.Y*c13y2*c23x2 -
            3*c13.X*c22x2*c13y2*c23.Y + c13x2*c13.Y*c23.X*(6*c21.Y*c23.Y + 3*c22y2) + c13x3*(-c21.Y*c23y2 -
            2*c22y2*c23.Y - c23.Y*(2*c21.Y*c23.Y + c22y2)),
        c11.X*c12.Y*c13.X*c13.Y*c23.X*c23.Y - c11.Y*c12.X*c13.X*c13.Y*c23.X*c23.Y + 6*c21.X*c22.X*c13y3*c23.X +
            3*c11.X*c12.X*c13.X*c13.Y*c23y2 + 6*c10.X*c13.X*c13y2*c23.X*c23.Y - 3*c11.X*c12.X*c13y2*c23.X*c23.Y -
            3*c11.Y*c12.Y*c13.X*c13.Y*c23x2 - 6*c10.Y*c13x2*c13.Y*c23.X*c23.Y - 6*c20.X*c13.X*c13y2*c23.X*c23.Y +
            3*c11.Y*c12.Y*c13x2*c23.X*c23.Y - 2*c12.X*c12y2*c13.X*c23.X*c23.Y - 6*c21.X*c13.X*c22.X*c13y2*c23.Y -
            6*c21.X*c13.X*c13y2*c22.Y*c23.X - 6*c13.X*c21.Y*c22.X*c13y2*c23.X + 6*c21.X*c13x2*c13.Y*c22.Y*c23.Y +
            2*c12x2*c12.Y*c13.Y*c23.X*c23.Y + c22x3*c13y3 - 3*c10.X*c13y3*c23x2 + 3*c10.Y*c13x3*c23y2 +
            3*c20.X*c13y3*c23x2 + c12y3*c13.X*c23x2 - c12x3*c13.Y*c23y2 - 3*c10.X*c13x2*c13.Y*c23y2 +
            3*c10.Y*c13.X*c13y2*c23x2 - 2*c11.X*c12.Y*c13x2*c23y2 + c11.X*c12.Y*c13y2*c23x2 - c11.Y*c12.X*c13x2*c23y2 +
            2*c11.Y*c12.X*c13y2*c23x2 + 3*c20.X*c13x2*c13.Y*c23y2 - c12.X*c12y2*c13.Y*c23x2 -
            3*c20.Y*c13.X*c13y2*c23x2 + c12x2*c12.Y*c13.X*c23y2 - 3*c13.X*c22x2*c13y2*c22.Y +
            c13x2*c13.Y*c23.X*(6*c20.Y*c23.Y + 6*c21.Y*c22.Y) + c13x2*c22.X*c13.Y*(6*c21.Y*c23.Y + 3*c22y2) +
            c13x3*(-2*c21.Y*c22.Y*c23.Y - c20.Y*c23y2 - c22.Y*(2*c21.Y*c23.Y + c22y2) - c23.Y*(2*c20.Y*c23.Y + 2*c21.Y*c22.Y)),
        6*c11.X*c12.X*c13.X*c13.Y*c22.Y*c23.Y + c11.X*c12.Y*c13.X*c22.X*c13.Y*c23.Y + c11.X*c12.Y*c13.X*c13.Y*c22.Y*c23.X -
            c11.Y*c12.X*c13.X*c22.X*c13.Y*c23.Y - c11.Y*c12.X*c13.X*c13.Y*c22.Y*c23.X - 6*c11.Y*c12.Y*c13.X*c22.X*c13.Y*c23.X -
            6*c10.X*c22.X*c13y3*c23.X + 6*c20.X*c22.X*c13y3*c23.X + 6*c10.Y*c13x3*c22.Y*c23.Y + 2*c12y3*c13.X*c22.X*c23.X -
            2*c12x3*c13.Y*c22.Y*c23.Y + 6*c10.X*c13.X*c22.X*c13y2*c23.Y + 6*c10.X*c13.X*c13y2*c22.Y*c23.X +
            6*c10.Y*c13.X*c22.X*c13y2*c23.X - 3*c11.X*c12.X*c22.X*c13y2*c23.Y - 3*c11.X*c12.X*c13y2*c22.Y*c23.X +
            2*c11.X*c12.Y*c22.X*c13y2*c23.X + 4*c11.Y*c12.X*c22.X*c13y2*c23.X - 6*c10.X*c13x2*c13.Y*c22.Y*c23.Y -
            6*c10.Y*c13x2*c22.X*c13.Y*c23.Y - 6*c10.Y*c13x2*c13.Y*c22.Y*c23.X - 4*c11.X*c12.Y*c13x2*c22.Y*c23.Y -
            6*c20.X*c13.X*c22.X*c13y2*c23.Y - 6*c20.X*c13.X*c13y2*c22.Y*c23.X - 2*c11.Y*c12.X*c13x2*c22.Y*c23.Y +
            3*c11.Y*c12.Y*c13x2*c22.X*c23.Y + 3*c11.Y*c12.Y*c13x2*c22.Y*c23.X - 2*c12.X*c12y2*c13.X*c22.X*c23.Y -
            2*c12.X*c12y2*c13.X*c22.Y*c23.X - 2*c12.X*c12y2*c22.X*c13.Y*c23.X - 6*c20.Y*c13.X*c22.X*c13y2*c23.X -
            6*c21.X*c13.X*c21.Y*c13y2*c23.X - 6*c21.X*c13.X*c22.X*c13y2*c22.Y + 6*c20.X*c13x2*c13.Y*c22.Y*c23.Y +
            2*c12x2*c12.Y*c13.X*c22.Y*c23.Y + 2*c12x2*c12.Y*c22.X*c13.Y*c23.Y + 2*c12x2*c12.Y*c13.Y*c22.Y*c23.X +
            3*c21.X*c22x2*c13y3 + 3*c21x2*c13y3*c23.X - 3*c13.X*c21.Y*c22x2*c13y2 - 3*c21x2*c13.X*c13y2*c23.Y +
            c13x2*c22.X*c13.Y*(6*c20.Y*c23.Y + 6*c21.Y*c22.Y) + c13x2*c13.Y*c23.X*(6*c20.Y*c22.Y + 3*c21y2) +
            c21.X*c13x2*c13.Y*(6*c21.Y*c23.Y + 3*c22y2) + c13x3*(-2*c20.Y*c22.Y*c23.Y - c23.Y*(2*c20.Y*c22.Y + c21y2) -
            c21.Y*(2*c21.Y*c23.Y + c22y2) - c22.Y*(2*c20.Y*c23.Y + 2*c21.Y*c22.Y)),
        c11.X*c21.X*c12.Y*c13.X*c13.Y*c23.Y + c11.X*c12.Y*c13.X*c21.Y*c13.Y*c23.X + c11.X*c12.Y*c13.X*c22.X*c13.Y*c22.Y -
            c11.Y*c12.X*c21.X*c13.X*c13.Y*c23.Y - c11.Y*c12.X*c13.X*c21.Y*c13.Y*c23.X - c11.Y*c12.X*c13.X*c22.X*c13.Y*c22.Y -
            6*c11.Y*c21.X*c12.Y*c13.X*c13.Y*c23.X - 6*c10.X*c21.X*c13y3*c23.X + 6*c20.X*c21.X*c13y3*c23.X +
            2*c21.X*c12y3*c13.X*c23.X + 6*c10.X*c21.X*c13.X*c13y2*c23.Y + 6*c10.X*c13.X*c21.Y*c13y2*c23.X +
            6*c10.X*c13.X*c22.X*c13y2*c22.Y + 6*c10.Y*c21.X*c13.X*c13y2*c23.X - 3*c11.X*c12.X*c21.X*c13y2*c23.Y -
            3*c11.X*c12.X*c21.Y*c13y2*c23.X - 3*c11.X*c12.X*c22.X*c13y2*c22.Y + 2*c11.X*c21.X*c12.Y*c13y2*c23.X +
            4*c11.Y*c12.X*c21.X*c13y2*c23.X - 6*c10.Y*c21.X*c13x2*c13.Y*c23.Y - 6*c10.Y*c13x2*c21.Y*c13.Y*c23.X -
            6*c10.Y*c13x2*c22.X*c13.Y*c22.Y - 6*c20.X*c21.X*c13.X*c13y2*c23.Y - 6*c20.X*c13.X*c21.Y*c13y2*c23.X -
            6*c20.X*c13.X*c22.X*c13y2*c22.Y + 3*c11.Y*c21.X*c12.Y*c13x2*c23.Y - 3*c11.Y*c12.Y*c13.X*c22x2*c13.Y +
            3*c11.Y*c12.Y*c13x2*c21.Y*c23.X + 3*c11.Y*c12.Y*c13x2*c22.X*c22.Y - 2*c12.X*c21.X*c12y2*c13.X*c23.Y -
            2*c12.X*c21.X*c12y2*c13.Y*c23.X - 2*c12.X*c12y2*c13.X*c21.Y*c23.X - 2*c12.X*c12y2*c13.X*c22.X*c22.Y -
            6*c20.Y*c21.X*c13.X*c13y2*c23.X - 6*c21.X*c13.X*c21.Y*c22.X*c13y2 + 6*c20.Y*c13x2*c21.Y*c13.Y*c23.X +
            2*c12x2*c21.X*c12.Y*c13.Y*c23.Y + 2*c12x2*c12.Y*c21.Y*c13.Y*c23.X + 2*c12x2*c12.Y*c22.X*c13.Y*c22.Y -
            3*c10.X*c22x2*c13y3 + 3*c20.X*c22x2*c13y3 + 3*c21x2*c22.X*c13y3 + c12y3*c13.X*c22x2 +
            3*c10.Y*c13.X*c22x2*c13y2 + c11.X*c12.Y*c22x2*c13y2 + 2*c11.Y*c12.X*c22x2*c13y2 -
            c12.X*c12y2*c22x2*c13.Y - 3*c20.Y*c13.X*c22x2*c13y2 - 3*c21x2*c13.X*c13y2*c22.Y +
            c12x2*c12.Y*c13.X*(2*c21.Y*c23.Y + c22y2) + c11.X*c12.X*c13.X*c13.Y*(6*c21.Y*c23.Y + 3*c22y2) +
            c21.X*c13x2*c13.Y*(6*c20.Y*c23.Y + 6*c21.Y*c22.Y) + c12x3*c13.Y*(-2*c21.Y*c23.Y - c22y2) +
            c10.Y*c13x3*(6*c21.Y*c23.Y + 3*c22y2) + c11.Y*c12.X*c13x2*(-2*c21.Y*c23.Y - c22y2) +
            c11.X*c12.Y*c13x2*(-4*c21.Y*c23.Y - 2*c22y2) + c10.X*c13x2*c13.Y*(-6*c21.Y*c23.Y - 3*c22y2) +
            c13x2*c22.X*c13.Y*(6*c20.Y*c22.Y + 3*c21y2) + c20.X*c13x2*c13.Y*(6*c21.Y*c23.Y + 3*c22y2) +
            c13x3*(-2*c20.Y*c21.Y*c23.Y - c22.Y*(2*c20.Y*c22.Y + c21y2) - c20.Y*(2*c21.Y*c23.Y + c22y2) -
            c21.Y*(2*c20.Y*c23.Y + 2*c21.Y*c22.Y)),
        -c10.X*c11.X*c12.Y*c13.X*c13.Y*c23.Y + c10.X*c11.Y*c12.X*c13.X*c13.Y*c23.Y + 6*c10.X*c11.Y*c12.Y*c13.X*c13.Y*c23.X -
            6*c10.Y*c11.X*c12.X*c13.X*c13.Y*c23.Y - c10.Y*c11.X*c12.Y*c13.X*c13.Y*c23.X + c10.Y*c11.Y*c12.X*c13.X*c13.Y*c23.X +
            c11.X*c11.Y*c12.X*c12.Y*c13.X*c23.Y - c11.X*c11.Y*c12.X*c12.Y*c13.Y*c23.X + c11.X*c20.X*c12.Y*c13.X*c13.Y*c23.Y +
            c11.X*c20.Y*c12.Y*c13.X*c13.Y*c23.X + c11.X*c21.X*c12.Y*c13.X*c13.Y*c22.Y + c11.X*c12.Y*c13.X*c21.Y*c22.X*c13.Y -
            c20.X*c11.Y*c12.X*c13.X*c13.Y*c23.Y - 6*c20.X*c11.Y*c12.Y*c13.X*c13.Y*c23.X - c11.Y*c12.X*c20.Y*c13.X*c13.Y*c23.X -
            c11.Y*c12.X*c21.X*c13.X*c13.Y*c22.Y - c11.Y*c12.X*c13.X*c21.Y*c22.X*c13.Y - 6*c11.Y*c21.X*c12.Y*c13.X*c22.X*c13.Y -
            6*c10.X*c20.X*c13y3*c23.X - 6*c10.X*c21.X*c22.X*c13y3 - 2*c10.X*c12y3*c13.X*c23.X + 6*c20.X*c21.X*c22.X*c13y3 +
            2*c20.X*c12y3*c13.X*c23.X + 2*c21.X*c12y3*c13.X*c22.X + 2*c10.Y*c12x3*c13.Y*c23.Y - 6*c10.X*c10.Y*c13.X*c13y2*c23.X +
            3*c10.X*c11.X*c12.X*c13y2*c23.Y - 2*c10.X*c11.X*c12.Y*c13y2*c23.X - 4*c10.X*c11.Y*c12.X*c13y2*c23.X +
            3*c10.Y*c11.X*c12.X*c13y2*c23.X + 6*c10.X*c10.Y*c13x2*c13.Y*c23.Y + 6*c10.X*c20.X*c13.X*c13y2*c23.Y -
            3*c10.X*c11.Y*c12.Y*c13x2*c23.Y + 2*c10.X*c12.X*c12y2*c13.X*c23.Y + 2*c10.X*c12.X*c12y2*c13.Y*c23.X +
            6*c10.X*c20.Y*c13.X*c13y2*c23.X + 6*c10.X*c21.X*c13.X*c13y2*c22.Y + 6*c10.X*c13.X*c21.Y*c22.X*c13y2 +
            4*c10.Y*c11.X*c12.Y*c13x2*c23.Y + 6*c10.Y*c20.X*c13.X*c13y2*c23.X + 2*c10.Y*c11.Y*c12.X*c13x2*c23.Y -
            3*c10.Y*c11.Y*c12.Y*c13x2*c23.X + 2*c10.Y*c12.X*c12y2*c13.X*c23.X + 6*c10.Y*c21.X*c13.X*c22.X*c13y2 -
            3*c11.X*c20.X*c12.X*c13y2*c23.Y + 2*c11.X*c20.X*c12.Y*c13y2*c23.X + c11.X*c11.Y*c12y2*c13.X*c23.X -
            3*c11.X*c12.X*c20.Y*c13y2*c23.X - 3*c11.X*c12.X*c21.X*c13y2*c22.Y - 3*c11.X*c12.X*c21.Y*c22.X*c13y2 +
            2*c11.X*c21.X*c12.Y*c22.X*c13y2 + 4*c20.X*c11.Y*c12.X*c13y2*c23.X + 4*c11.Y*c12.X*c21.X*c22.X*c13y2 -
            2*c10.X*c12x2*c12.Y*c13.Y*c23.Y - 6*c10.Y*c20.X*c13x2*c13.Y*c23.Y - 6*c10.Y*c20.Y*c13x2*c13.Y*c23.X -
            6*c10.Y*c21.X*c13x2*c13.Y*c22.Y - 2*c10.Y*c12x2*c12.Y*c13.X*c23.Y - 2*c10.Y*c12x2*c12.Y*c13.Y*c23.X -
            6*c10.Y*c13x2*c21.Y*c22.X*c13.Y - c11.X*c11.Y*c12x2*c13.Y*c23.Y - 2*c11.X*c11y2*c13.X*c13.Y*c23.X +
            3*c20.X*c11.Y*c12.Y*c13x2*c23.Y - 2*c20.X*c12.X*c12y2*c13.X*c23.Y - 2*c20.X*c12.X*c12y2*c13.Y*c23.X -
            6*c20.X*c20.Y*c13.X*c13y2*c23.X - 6*c20.X*c21.X*c13.X*c13y2*c22.Y - 6*c20.X*c13.X*c21.Y*c22.X*c13y2 +
            3*c11.Y*c20.Y*c12.Y*c13x2*c23.X + 3*c11.Y*c21.X*c12.Y*c13x2*c22.Y + 3*c11.Y*c12.Y*c13x2*c21.Y*c22.X -
            2*c12.X*c20.Y*c12y2*c13.X*c23.X - 2*c12.X*c21.X*c12y2*c13.X*c22.Y - 2*c12.X*c21.X*c12y2*c22.X*c13.Y -
            2*c12.X*c12y2*c13.X*c21.Y*c22.X - 6*c20.Y*c21.X*c13.X*c22.X*c13y2 - c11y2*c12.X*c12.Y*c13.X*c23.X +
            2*c20.X*c12x2*c12.Y*c13.Y*c23.Y + 6*c20.Y*c13x2*c21.Y*c22.X*c13.Y + 2*c11x2*c11.Y*c13.X*c13.Y*c23.Y +
            c11x2*c12.X*c12.Y*c13.Y*c23.Y + 2*c12x2*c20.Y*c12.Y*c13.Y*c23.X + 2*c12x2*c21.X*c12.Y*c13.Y*c22.Y +
            2*c12x2*c12.Y*c21.Y*c22.X*c13.Y + c21x3*c13y3 + 3*c10x2*c13y3*c23.X - 3*c10y2*c13x3*c23.Y +
            3*c20x2*c13y3*c23.X + c11y3*c13x2*c23.X - c11x3*c13y2*c23.Y - c11.X*c11y2*c13x2*c23.Y +
            c11x2*c11.Y*c13y2*c23.X - 3*c10x2*c13.X*c13y2*c23.Y + 3*c10y2*c13x2*c13.Y*c23.X - c11x2*c12y2*c13.X*c23.Y +
            c11y2*c12x2*c13.Y*c23.X - 3*c21x2*c13.X*c21.Y*c13y2 - 3*c20x2*c13.X*c13y2*c23.Y + 3*c20y2*c13x2*c13.Y*c23.X +
            c11.X*c12.X*c13.X*c13.Y*(6*c20.Y*c23.Y + 6*c21.Y*c22.Y) + c12x3*c13.Y*(-2*c20.Y*c23.Y - 2*c21.Y*c22.Y) +
            c10.Y*c13x3*(6*c20.Y*c23.Y + 6*c21.Y*c22.Y) + c11.Y*c12.X*c13x2*(-2*c20.Y*c23.Y - 2*c21.Y*c22.Y) +
            c12x2*c12.Y*c13.X*(2*c20.Y*c23.Y + 2*c21.Y*c22.Y) + c11.X*c12.Y*c13x2*(-4*c20.Y*c23.Y - 4*c21.Y*c22.Y) +
            c10.X*c13x2*c13.Y*(-6*c20.Y*c23.Y - 6*c21.Y*c22.Y) + c20.X*c13x2*c13.Y*(6*c20.Y*c23.Y + 6*c21.Y*c22.Y) +
            c21.X*c13x2*c13.Y*(6*c20.Y*c22.Y + 3*c21y2) + c13x3*(-2*c20.Y*c21.Y*c22.Y - c20y2*c23.Y -
            c21.Y*(2*c20.Y*c22.Y + c21y2) - c20.Y*(2*c20.Y*c23.Y + 2*c21.Y*c22.Y)),
        -c10.X*c11.X*c12.Y*c13.X*c13.Y*c22.Y + c10.X*c11.Y*c12.X*c13.X*c13.Y*c22.Y + 6*c10.X*c11.Y*c12.Y*c13.X*c22.X*c13.Y -
            6*c10.Y*c11.X*c12.X*c13.X*c13.Y*c22.Y - c10.Y*c11.X*c12.Y*c13.X*c22.X*c13.Y + c10.Y*c11.Y*c12.X*c13.X*c22.X*c13.Y +
            c11.X*c11.Y*c12.X*c12.Y*c13.X*c22.Y - c11.X*c11.Y*c12.X*c12.Y*c22.X*c13.Y + c11.X*c20.X*c12.Y*c13.X*c13.Y*c22.Y +
            c11.X*c20.Y*c12.Y*c13.X*c22.X*c13.Y + c11.X*c21.X*c12.Y*c13.X*c21.Y*c13.Y - c20.X*c11.Y*c12.X*c13.X*c13.Y*c22.Y -
            6*c20.X*c11.Y*c12.Y*c13.X*c22.X*c13.Y - c11.Y*c12.X*c20.Y*c13.X*c22.X*c13.Y - c11.Y*c12.X*c21.X*c13.X*c21.Y*c13.Y -
            6*c10.X*c20.X*c22.X*c13y3 - 2*c10.X*c12y3*c13.X*c22.X + 2*c20.X*c12y3*c13.X*c22.X + 2*c10.Y*c12x3*c13.Y*c22.Y -
            6*c10.X*c10.Y*c13.X*c22.X*c13y2 + 3*c10.X*c11.X*c12.X*c13y2*c22.Y - 2*c10.X*c11.X*c12.Y*c22.X*c13y2 -
            4*c10.X*c11.Y*c12.X*c22.X*c13y2 + 3*c10.Y*c11.X*c12.X*c22.X*c13y2 + 6*c10.X*c10.Y*c13x2*c13.Y*c22.Y +
            6*c10.X*c20.X*c13.X*c13y2*c22.Y - 3*c10.X*c11.Y*c12.Y*c13x2*c22.Y + 2*c10.X*c12.X*c12y2*c13.X*c22.Y +
            2*c10.X*c12.X*c12y2*c22.X*c13.Y + 6*c10.X*c20.Y*c13.X*c22.X*c13y2 + 6*c10.X*c21.X*c13.X*c21.Y*c13y2 +
            4*c10.Y*c11.X*c12.Y*c13x2*c22.Y + 6*c10.Y*c20.X*c13.X*c22.X*c13y2 + 2*c10.Y*c11.Y*c12.X*c13x2*c22.Y -
            3*c10.Y*c11.Y*c12.Y*c13x2*c22.X + 2*c10.Y*c12.X*c12y2*c13.X*c22.X - 3*c11.X*c20.X*c12.X*c13y2*c22.Y +
            2*c11.X*c20.X*c12.Y*c22.X*c13y2 + c11.X*c11.Y*c12y2*c13.X*c22.X - 3*c11.X*c12.X*c20.Y*c22.X*c13y2 -
            3*c11.X*c12.X*c21.X*c21.Y*c13y2 + 4*c20.X*c11.Y*c12.X*c22.X*c13y2 - 2*c10.X*c12x2*c12.Y*c13.Y*c22.Y -
            6*c10.Y*c20.X*c13x2*c13.Y*c22.Y - 6*c10.Y*c20.Y*c13x2*c22.X*c13.Y - 6*c10.Y*c21.X*c13x2*c21.Y*c13.Y -
            2*c10.Y*c12x2*c12.Y*c13.X*c22.Y - 2*c10.Y*c12x2*c12.Y*c22.X*c13.Y - c11.X*c11.Y*c12x2*c13.Y*c22.Y -
            2*c11.X*c11y2*c13.X*c22.X*c13.Y + 3*c20.X*c11.Y*c12.Y*c13x2*c22.Y - 2*c20.X*c12.X*c12y2*c13.X*c22.Y -
            2*c20.X*c12.X*c12y2*c22.X*c13.Y - 6*c20.X*c20.Y*c13.X*c22.X*c13y2 - 6*c20.X*c21.X*c13.X*c21.Y*c13y2 +
            3*c11.Y*c20.Y*c12.Y*c13x2*c22.X + 3*c11.Y*c21.X*c12.Y*c13x2*c21.Y - 2*c12.X*c20.Y*c12y2*c13.X*c22.X -
            2*c12.X*c21.X*c12y2*c13.X*c21.Y - c11y2*c12.X*c12.Y*c13.X*c22.X + 2*c20.X*c12x2*c12.Y*c13.Y*c22.Y -
            3*c11.Y*c21x2*c12.Y*c13.X*c13.Y + 6*c20.Y*c21.X*c13x2*c21.Y*c13.Y + 2*c11x2*c11.Y*c13.X*c13.Y*c22.Y +
            c11x2*c12.X*c12.Y*c13.Y*c22.Y + 2*c12x2*c20.Y*c12.Y*c22.X*c13.Y + 2*c12x2*c21.X*c12.Y*c21.Y*c13.Y -
            3*c10.X*c21x2*c13y3 + 3*c20.X*c21x2*c13y3 + 3*c10x2*c22.X*c13y3 - 3*c10y2*c13x3*c22.Y + 3*c20x2*c22.X*c13y3 +
            c21x2*c12y3*c13.X + c11y3*c13x2*c22.X - c11x3*c13y2*c22.Y + 3*c10.Y*c21x2*c13.X*c13y2 -
            c11.X*c11y2*c13x2*c22.Y + c11.X*c21x2*c12.Y*c13y2 + 2*c11.Y*c12.X*c21x2*c13y2 + c11x2*c11.Y*c22.X*c13y2 -
            c12.X*c21x2*c12y2*c13.Y - 3*c20.Y*c21x2*c13.X*c13y2 - 3*c10x2*c13.X*c13y2*c22.Y + 3*c10y2*c13x2*c22.X*c13.Y -
            c11x2*c12y2*c13.X*c22.Y + c11y2*c12x2*c22.X*c13.Y - 3*c20x2*c13.X*c13y2*c22.Y + 3*c20y2*c13x2*c22.X*c13.Y +
            c12x2*c12.Y*c13.X*(2*c20.Y*c22.Y + c21y2) + c11.X*c12.X*c13.X*c13.Y*(6*c20.Y*c22.Y + 3*c21y2) +
            c12x3*c13.Y*(-2*c20.Y*c22.Y - c21y2) + c10.Y*c13x3*(6*c20.Y*c22.Y + 3*c21y2) +
            c11.Y*c12.X*c13x2*(-2*c20.Y*c22.Y - c21y2) + c11.X*c12.Y*c13x2*(-4*c20.Y*c22.Y - 2*c21y2) +
            c10.X*c13x2*c13.Y*(-6*c20.Y*c22.Y - 3*c21y2) + c20.X*c13x2*c13.Y*(6*c20.Y*c22.Y + 3*c21y2) +
            c13x3*(-2*c20.Y*c21y2 - c20y2*c22.Y - c20.Y*(2*c20.Y*c22.Y + c21y2)),
        -c10.X*c11.X*c12.Y*c13.X*c21.Y*c13.Y + c10.X*c11.Y*c12.X*c13.X*c21.Y*c13.Y + 6*c10.X*c11.Y*c21.X*c12.Y*c13.X*c13.Y -
            6*c10.Y*c11.X*c12.X*c13.X*c21.Y*c13.Y - c10.Y*c11.X*c21.X*c12.Y*c13.X*c13.Y + c10.Y*c11.Y*c12.X*c21.X*c13.X*c13.Y -
            c11.X*c11.Y*c12.X*c21.X*c12.Y*c13.Y + c11.X*c11.Y*c12.X*c12.Y*c13.X*c21.Y + c11.X*c20.X*c12.Y*c13.X*c21.Y*c13.Y +
            6*c11.X*c12.X*c20.Y*c13.X*c21.Y*c13.Y + c11.X*c20.Y*c21.X*c12.Y*c13.X*c13.Y - c20.X*c11.Y*c12.X*c13.X*c21.Y*c13.Y -
            6*c20.X*c11.Y*c21.X*c12.Y*c13.X*c13.Y - c11.Y*c12.X*c20.Y*c21.X*c13.X*c13.Y - 6*c10.X*c20.X*c21.X*c13y3 -
            2*c10.X*c21.X*c12y3*c13.X + 6*c10.Y*c20.Y*c13x3*c21.Y + 2*c20.X*c21.X*c12y3*c13.X + 2*c10.Y*c12x3*c21.Y*c13.Y -
            2*c12x3*c20.Y*c21.Y*c13.Y - 6*c10.X*c10.Y*c21.X*c13.X*c13y2 + 3*c10.X*c11.X*c12.X*c21.Y*c13y2 -
            2*c10.X*c11.X*c21.X*c12.Y*c13y2 - 4*c10.X*c11.Y*c12.X*c21.X*c13y2 + 3*c10.Y*c11.X*c12.X*c21.X*c13y2 +
            6*c10.X*c10.Y*c13x2*c21.Y*c13.Y + 6*c10.X*c20.X*c13.X*c21.Y*c13y2 - 3*c10.X*c11.Y*c12.Y*c13x2*c21.Y +
            2*c10.X*c12.X*c21.X*c12y2*c13.Y + 2*c10.X*c12.X*c12y2*c13.X*c21.Y + 6*c10.X*c20.Y*c21.X*c13.X*c13y2 +
            4*c10.Y*c11.X*c12.Y*c13x2*c21.Y + 6*c10.Y*c20.X*c21.X*c13.X*c13y2 + 2*c10.Y*c11.Y*c12.X*c13x2*c21.Y -
            3*c10.Y*c11.Y*c21.X*c12.Y*c13x2 + 2*c10.Y*c12.X*c21.X*c12y2*c13.X - 3*c11.X*c20.X*c12.X*c21.Y*c13y2 +
            2*c11.X*c20.X*c21.X*c12.Y*c13y2 + c11.X*c11.Y*c21.X*c12y2*c13.X - 3*c11.X*c12.X*c20.Y*c21.X*c13y2 +
            4*c20.X*c11.Y*c12.X*c21.X*c13y2 - 6*c10.X*c20.Y*c13x2*c21.Y*c13.Y - 2*c10.X*c12x2*c12.Y*c21.Y*c13.Y -
            6*c10.Y*c20.X*c13x2*c21.Y*c13.Y - 6*c10.Y*c20.Y*c21.X*c13x2*c13.Y - 2*c10.Y*c12x2*c21.X*c12.Y*c13.Y -
            2*c10.Y*c12x2*c12.Y*c13.X*c21.Y - c11.X*c11.Y*c12x2*c21.Y*c13.Y - 4*c11.X*c20.Y*c12.Y*c13x2*c21.Y -
            2*c11.X*c11y2*c21.X*c13.X*c13.Y + 3*c20.X*c11.Y*c12.Y*c13x2*c21.Y - 2*c20.X*c12.X*c21.X*c12y2*c13.Y -
            2*c20.X*c12.X*c12y2*c13.X*c21.Y - 6*c20.X*c20.Y*c21.X*c13.X*c13y2 - 2*c11.Y*c12.X*c20.Y*c13x2*c21.Y +
            3*c11.Y*c20.Y*c21.X*c12.Y*c13x2 - 2*c12.X*c20.Y*c21.X*c12y2*c13.X - c11y2*c12.X*c21.X*c12.Y*c13.X +
            6*c20.X*c20.Y*c13x2*c21.Y*c13.Y + 2*c20.X*c12x2*c12.Y*c21.Y*c13.Y + 2*c11x2*c11.Y*c13.X*c21.Y*c13.Y +
            c11x2*c12.X*c12.Y*c21.Y*c13.Y + 2*c12x2*c20.Y*c21.X*c12.Y*c13.Y + 2*c12x2*c20.Y*c12.Y*c13.X*c21.Y +
            3*c10x2*c21.X*c13y3 - 3*c10y2*c13x3*c21.Y + 3*c20x2*c21.X*c13y3 + c11y3*c21.X*c13x2 - c11x3*c21.Y*c13y2 -
            3*c20y2*c13x3*c21.Y - c11.X*c11y2*c13x2*c21.Y + c11x2*c11.Y*c21.X*c13y2 - 3*c10x2*c13.X*c21.Y*c13y2 +
            3*c10y2*c21.X*c13x2*c13.Y - c11x2*c12y2*c13.X*c21.Y + c11y2*c12x2*c21.X*c13.Y - 3*c20x2*c13.X*c21.Y*c13y2 +
            3*c20y2*c21.X*c13x2*c13.Y,
        c10.X*c10.Y*c11.X*c12.Y*c13.X*c13.Y - c10.X*c10.Y*c11.Y*c12.X*c13.X*c13.Y + c10.X*c11.X*c11.Y*c12.X*c12.Y*c13.Y -
            c10.Y*c11.X*c11.Y*c12.X*c12.Y*c13.X - c10.X*c11.X*c20.Y*c12.Y*c13.X*c13.Y + 6*c10.X*c20.X*c11.Y*c12.Y*c13.X*c13.Y +
            c10.X*c11.Y*c12.X*c20.Y*c13.X*c13.Y - c10.Y*c11.X*c20.X*c12.Y*c13.X*c13.Y - 6*c10.Y*c11.X*c12.X*c20.Y*c13.X*c13.Y +
            c10.Y*c20.X*c11.Y*c12.X*c13.X*c13.Y - c11.X*c20.X*c11.Y*c12.X*c12.Y*c13.Y + c11.X*c11.Y*c12.X*c20.Y*c12.Y*c13.X +
            c11.X*c20.X*c20.Y*c12.Y*c13.X*c13.Y - c20.X*c11.Y*c12.X*c20.Y*c13.X*c13.Y - 2*c10.X*c20.X*c12y3*c13.X +
            2*c10.Y*c12x3*c20.Y*c13.Y - 3*c10.X*c10.Y*c11.X*c12.X*c13y2 - 6*c10.X*c10.Y*c20.X*c13.X*c13y2 +
            3*c10.X*c10.Y*c11.Y*c12.Y*c13x2 - 2*c10.X*c10.Y*c12.X*c12y2*c13.X - 2*c10.X*c11.X*c20.X*c12.Y*c13y2 -
            c10.X*c11.X*c11.Y*c12y2*c13.X + 3*c10.X*c11.X*c12.X*c20.Y*c13y2 - 4*c10.X*c20.X*c11.Y*c12.X*c13y2 +
            3*c10.Y*c11.X*c20.X*c12.X*c13y2 + 6*c10.X*c10.Y*c20.Y*c13x2*c13.Y + 2*c10.X*c10.Y*c12x2*c12.Y*c13.Y +
            2*c10.X*c11.X*c11y2*c13.X*c13.Y + 2*c10.X*c20.X*c12.X*c12y2*c13.Y + 6*c10.X*c20.X*c20.Y*c13.X*c13y2 -
            3*c10.X*c11.Y*c20.Y*c12.Y*c13x2 + 2*c10.X*c12.X*c20.Y*c12y2*c13.X + c10.X*c11y2*c12.X*c12.Y*c13.X +
            c10.Y*c11.X*c11.Y*c12x2*c13.Y + 4*c10.Y*c11.X*c20.Y*c12.Y*c13x2 - 3*c10.Y*c20.X*c11.Y*c12.Y*c13x2 +
            2*c10.Y*c20.X*c12.X*c12y2*c13.X + 2*c10.Y*c11.Y*c12.X*c20.Y*c13x2 + c11.X*c20.X*c11.Y*c12y2*c13.X -
            3*c11.X*c20.X*c12.X*c20.Y*c13y2 - 2*c10.X*c12x2*c20.Y*c12.Y*c13.Y - 6*c10.Y*c20.X*c20.Y*c13x2*c13.Y -
            2*c10.Y*c20.X*c12x2*c12.Y*c13.Y - 2*c10.Y*c11x2*c11.Y*c13.X*c13.Y - c10.Y*c11x2*c12.X*c12.Y*c13.Y -
            2*c10.Y*c12x2*c20.Y*c12.Y*c13.X - 2*c11.X*c20.X*c11y2*c13.X*c13.Y - c11.X*c11.Y*c12x2*c20.Y*c13.Y +
            3*c20.X*c11.Y*c20.Y*c12.Y*c13x2 - 2*c20.X*c12.X*c20.Y*c12y2*c13.X - c20.X*c11y2*c12.X*c12.Y*c13.X +
            3*c10y2*c11.X*c12.X*c13.X*c13.Y + 3*c11.X*c12.X*c20y2*c13.X*c13.Y + 2*c20.X*c12x2*c20.Y*c12.Y*c13.Y -
            3*c10x2*c11.Y*c12.Y*c13.X*c13.Y + 2*c11x2*c11.Y*c20.Y*c13.X*c13.Y + c11x2*c12.X*c20.Y*c12.Y*c13.Y -
            3*c20x2*c11.Y*c12.Y*c13.X*c13.Y - c10x3*c13y3 + c10y3*c13x3 + c20x3*c13y3 - c20y3*c13x3 -
            3*c10.X*c20x2*c13y3 - c10.X*c11y3*c13x2 + 3*c10x2*c20.X*c13y3 + c10.Y*c11x3*c13y2 +
            3*c10.Y*c20y2*c13x3 + c20.X*c11y3*c13x2 + c10x2*c12y3*c13.X - 3*c10y2*c20.Y*c13x3 - c10y2*c12x3*c13.Y +
            c20x2*c12y3*c13.X - c11x3*c20.Y*c13y2 - c12x3*c20y2*c13.Y - c10.X*c11x2*c11.Y*c13y2 +
            c10.Y*c11.X*c11y2*c13x2 - 3*c10.X*c10y2*c13x2*c13.Y - c10.X*c11y2*c12x2*c13.Y + c10.Y*c11x2*c12y2*c13.X -
            c11.X*c11y2*c20.Y*c13x2 + 3*c10x2*c10.Y*c13.X*c13y2 + c10x2*c11.X*c12.Y*c13y2 +
            2*c10x2*c11.Y*c12.X*c13y2 - 2*c10y2*c11.X*c12.Y*c13x2 - c10y2*c11.Y*c12.X*c13x2 + c11x2*c20.X*c11.Y*c13y2 -
            3*c10.X*c20y2*c13x2*c13.Y + 3*c10.Y*c20x2*c13.X*c13y2 + c11.X*c20x2*c12.Y*c13y2 - 2*c11.X*c20y2*c12.Y*c13x2 +
            c20.X*c11y2*c12x2*c13.Y - c11.Y*c12.X*c20y2*c13x2 - c10x2*c12.X*c12y2*c13.Y - 3*c10x2*c20.Y*c13.X*c13y2 +
            3*c10y2*c20.X*c13x2*c13.Y + c10y2*c12x2*c12.Y*c13.X - c11x2*c20.Y*c12y2*c13.X + 2*c20x2*c11.Y*c12.X*c13y2 +
            3*c20.X*c20y2*c13x2*c13.Y - c20x2*c12.X*c12y2*c13.Y - 3*c20x2*c20.Y*c13.X*c13y2 + c12x2*c20y2*c12.Y*c13.X
    );
    var roots = poly.getRootsInInterval(0,1);

    for ( var i = 0; i < roots.Length; i++ ) {
        var s = roots[i];
        var xRoots = new Polynomial(
            c13.X,
            c12.X,
            c11.X,
            c10.X - c20.X - s*c21.X - s*s*c22.X - s*s*s*c23.X
        ).getRoots();
        var yRoots = new Polynomial(
            c13.Y,
            c12.Y,
            c11.Y,
            c10.Y - c20.Y - s*c21.Y - s*s*c22.Y - s*s*s*c23.Y
        ).getRoots();

        if ( xRoots.Length > 0 && yRoots.Length > 0 ) {
            var TOLERANCE = 1e-4;

            checkRoots:
            for ( var j = 0; j < xRoots.Length; j++ ) {
                var xRoot = xRoots[j];
                
                if ( 0 <= xRoot && xRoot <= 1 ) {
                    for ( var k = 0; k < yRoots.Length; k++ ) {
                        if ( Math.abs( xRoot - yRoots[k] ) < TOLERANCE ) {
                            result.points.push(
                                c23.multiply(s*s*s).add(c22.multiply(s*s).add(c21.multiply(s).add(c20)))
                            );
                            break checkRoots;
                        }
                    }
                }
            }
        }
    }

    if ( result.points.Length > 0 ) result.status = "Intersection";

    return result;
}


/*****
*
*   intersectBezier3Circle
*
*****/
public static Intersection intersectBezier3Circle(Point p1, Point p2, Point p3, Point p4, Point c, double r) {
    return Intersection.intersectBezier3Ellipse(p1, p2, p3, p4, c, r, r);
}


/*****
*
*   intersectBezier3Ellipse
*
*****/
public static Intersection intersectBezier3Ellipse(Point p1, Point p2, Point p3, Point p4, Point ec, double rx, double ry) {
    Point a, b, c, d;       // temporary variables
    Vector2D c3, c2, c1, c0;   // coefficients of cubic
    var result = new Intersection("No Intersection");

    // Calculate the coefficients of cubic polynomial
    a = p1.multiply(-1);
    b = p2.multiply(3);
    c = p3.multiply(-3);
    d = a.add(b.add(c.add(p4)));
    c3 = new Vector2D(d.X, d.Y);

    a = p1.multiply(3);
    b = p2.multiply(-6);
    c = p3.multiply(3);
    d = a.add(b.add(c));
    c2 = new Vector2D(d.X, d.Y);

    a = p1.multiply(-3);
    b = p2.multiply(3);
    c = a.add(b);
    c1 = new Vector2D(c.X, c.Y);

    c0 = new Vector2D(p1.X, p1.Y);

    var rxrx  = rx*rx;
    var ryry  = ry*ry;
    var poly = new Polynomial(
        c3.X*c3.X*ryry + c3.Y*c3.Y*rxrx,
        2*(c3.X*c2.X*ryry + c3.Y*c2.Y*rxrx),
        2*(c3.X*c1.X*ryry + c3.Y*c1.Y*rxrx) + c2.X*c2.X*ryry + c2.Y*c2.Y*rxrx,
        2*c3.X*ryry*(c0.X - ec.X) + 2*c3.Y*rxrx*(c0.Y - ec.Y) +
            2*(c2.X*c1.X*ryry + c2.Y*c1.Y*rxrx),
        2*c2.X*ryry*(c0.X - ec.X) + 2*c2.Y*rxrx*(c0.Y - ec.Y) +
            c1.X*c1.X*ryry + c1.Y*c1.Y*rxrx,
        2*c1.X*ryry*(c0.X - ec.X) + 2*c1.Y*rxrx*(c0.Y - ec.Y),
        c0.X*c0.X*ryry - 2*c0.Y*ec.Y*rxrx - 2*c0.X*ec.X*ryry +
            c0.Y*c0.Y*rxrx + ec.X*ec.X*ryry + ec.Y*ec.Y*rxrx - rxrx*ryry
    );
    var roots = poly.getRootsInInterval(0,1);

    for ( var i = 0; i < roots.Length; i++ ) {
        var t = roots[i];

        result.points.push(
            c3.multiply(t*t*t).add(c2.multiply(t*t).add(c1.multiply(t).add(c0)))
        );
    }

    if ( result.points.Length > 0 ) result.status = "Intersection";

    return result;
}


/*****
*
*   intersectBezier3Line
*
*   Many thanks to Dan Sunday at SoftSurfer.com.  He gave me a very thorough
*   sketch of the algorithm used here.  Without his help, I'm not sure when I
*   would have figured out this intersection problem.
*
*****/
public static Intersection intersectBezier3Line(Point p1, Point p2, Point p3, Point p4, Point a1, Point a2) {
    Point a, b, c, d;       // temporary variables
    Vector2D c3, c2, c1, c0;   // coefficients of cubic
    Point cl;               // c coefficient for normal form of line
    Vector2D n;                // normal for normal form of line
    var min = a1.min(a2); // used to determine if point is on line segment
    var max = a1.max(a2); // used to determine if point is on line segment
    var result = new Intersection("No Intersection");
    
    // Start with Bezier using Bernstein polynomials for weighting functions:
    //     (1-t^3)P1 + 3t(1-t)^2P2 + 3t^2(1-t)P3 + t^3P4
    //
    // Expand and collect terms to form linear combinations of original Bezier
    // controls.  This ends up with a vector cubic in t:
    //     (-P1+3P2-3P3+P4)t^3 + (3P1-6P2+3P3)t^2 + (-3P1+3P2)t + P1
    //             /\                  /\                /\       /\
    //             ||                  ||                ||       ||
    //             c3                  c2                c1       c0
    
    // Calculate the coefficients
    a = p1.multiply(-1);
    b = p2.multiply(3);
    c = p3.multiply(-3);
    d = a.add(b.add(c.add(p4)));
    c3 = new Vector2D(d.X, d.Y);

    a = p1.multiply(3);
    b = p2.multiply(-6);
    c = p3.multiply(3);
    d = a.add(b.add(c));
    c2 = new Vector2D(d.X, d.Y);

    a = p1.multiply(-3);
    b = p2.multiply(3);
    c = a.add(b);
    c1 = new Vector2D(c.X, c.Y);

    c0 = new Vector2D(p1.X, p1.Y);
    
    // Convert line to normal form: ax + by + c = 0
    // Find normal to line: negative inverse of original line's slope
    n = new Vector2D(a1.Y - a2.Y, a2.X - a1.X);
    
    // Determine new c coefficient
    cl = a1.X*a2.Y - a2.X*a1.Y;

    // ?Rotate each cubic coefficient using line for new coordinate system?
    // Find roots of rotated cubic
    roots = new Polynomial(
        n.dot(c3),
        n.dot(c2),
        n.dot(c1),
        n.dot(c0) + cl
    ).getRoots();

    // Any roots in closed interval [0,1] are intersections on Bezier, but
    // might not be on the line segment.
    // Find intersections and calculate point coordinates
    for ( var i = 0; i < roots.Length; i++ ) {
        var t = roots[i];

        if ( 0 <= t && t <= 1 ) {
            // We're within the Bezier curve
            // Find point on Bezier
            var p5 = p1.lerp(p2, t);
            var p6 = p2.lerp(p3, t);
            var p7 = p3.lerp(p4, t);

            var p8 = p5.lerp(p6, t);
            var p9 = p6.lerp(p7, t);

            var p10 = p8.lerp(p9, t);

            // See if point is on line segment
            // Had to make special cases for vertical and horizontal lines due
            // to slight errors in calculation of p10
            if ( a1.X == a2.X ) {
                if ( min.Y <= p10.Y && p10.Y <= max.Y ) {
                    result.status = "Intersection";
                    result.appendPoint( p10 );
                }
            } else if ( a1.Y == a2.Y ) {
                if ( min.X <= p10.X && p10.X <= max.X ) {
                    result.status = "Intersection";
                    result.appendPoint( p10 );
                }
            } else if ( p10.gte(min) && p10.lte(max) ) {
                result.status = "Intersection";
                result.appendPoint( p10 );
            }
        }
    }

    return result;
}


/*****
*
*   intersectBezier3Polygon
*
*****/
public static Intersection intersectBezier3Polygon(p1, p2, p3, p4, points) {
    var result = new Intersection("No Intersection");
    var length = points.Length;

    for ( var i = 0; i < length; i++ ) {
        var a1 = points[i];
        var a2 = points[(i+1) % length];
        var inter = Intersection.intersectBezier3Line(p1, p2, p3, p4, a1, a2);

        result.appendPoints(inter.points);
    }

    if ( result.points.Length > 0 ) result.status = "Intersection";

    return result;
}


/*****
*
*   intersectBezier3Rectangle
*
*****/
public static Intersection intersectBezier3Rectangle(Point p1, Point p2, Point p3, Point p4, Point r1, Point r2) {
    var min        = r1.min(r2);
    var max        = r1.max(r2);
    var topRight   = new Point2D( max.X, min.Y );
    var bottomLeft = new Point2D( min.X, max.Y );
    
    var inter1 = Intersection.intersectBezier3Line(p1, p2, p3, p4, min, topRight);
    var inter2 = Intersection.intersectBezier3Line(p1, p2, p3, p4, topRight, max);
    var inter3 = Intersection.intersectBezier3Line(p1, p2, p3, p4, max, bottomLeft);
    var inter4 = Intersection.intersectBezier3Line(p1, p2, p3, p4, bottomLeft, min);
    
    var result = new Intersection("No Intersection");

    result.appendPoints(inter1.points);
    result.appendPoints(inter2.points);
    result.appendPoints(inter3.points);
    result.appendPoints(inter4.points);

    if ( result.points.Length > 0 ) result.status = "Intersection";

    return result;
}




/*****
*
*   intersectRayRay
*
*****/
public static Intersection intersectRayRay(Point a1, Point a2, Point b1, Point b2) {
    var result;
    
    var ua_t = (b2.X - b1.X) * (a1.Y - b1.Y) - (b2.Y - b1.Y) * (a1.X - b1.X);
    var ub_t = (a2.X - a1.X) * (a1.Y - b1.Y) - (a2.Y - a1.Y) * (a1.X - b1.X);
    var u_b  = (b2.Y - b1.Y) * (a2.X - a1.X) - (b2.X - b1.X) * (a2.Y - a1.Y);

    if ( u_b != 0 ) {
        var ua = ua_t / u_b;

        result = new Intersection("Intersection");
        result.points.push(
            new Point2D(
                a1.X + ua * (a2.X - a1.X),
                a1.Y + ua * (a2.Y - a1.Y)
            )
        );
    } else {
        if ( ua_t == 0 || ub_t == 0 ) {
            result = new Intersection("Coincident");
        } else {
            result = new Intersection("Parallel");
        }
    }

    return result;
}





    }
}
