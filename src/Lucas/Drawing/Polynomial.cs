using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucas
{
    public class Polynomial
    {
        const double TOLERANCE = 1e-6;
        const double ACCURACY = 6;

        public Polynomial(params double[] args)
        {
            this.init(args);
        }

        public List<double> coefs { get; set; }

        private void init(params double[] coefs)
        {
            this.coefs = new List<double>();
            for (var i = coefs.Length - 1; i >= 0; i--)
                this.coefs.Add(coefs[i]);
        }

        //double eval(double x){double result=0;for(var i=this.coefs.Count-1;i>=0;i--)result=result*x+this.coefs[i];return result;}
        //Polynomial multiply(Polynomial that){var result=new Polynomial();for(var i=0;i<=this.getDegree()+that.getDegree();i++)result.coefs.Add(0);for(var i=0;i<=this.getDegree();i++)for(var j=0;j<=that.getDegree();j++)result.coefs[i+j]+=this.coefs[i]*that.coefs[j];return result;};
        //void divide_scalar(double scalar){for(var i=0;i<this.coefs.Count;i++)this.coefs[i]/=scalar;}
        void simplify()
        {
            for (var i = this.getDegree(); i >= 0; i--)
            {
                if (Math.Abs(this.coefs[i]) <= Polynomial.TOLERANCE)
                    this.coefs.RemoveAt(this.coefs.Count - 1);
                else
                    break;
            }
        }
        //double bisection(double min,double max){var minValue=this.eval(min);var maxValue=this.eval(max);double result;if(Math.Abs(minValue)<=Polynomial.TOLERANCE)result=min;else if(Math.Abs(maxValue)<=Polynomial.TOLERANCE)result=max;else if(minValue*maxValue<=0){var tmp1=Math.Log(max-min);var tmp2=Math.Log(10)*Polynomial.ACCURACY;var iters=Math.ceil((tmp1+tmp2)/Math.Log(2));for(var i=0;i<iters;i++){result=0.5*(min+max);var value=this.eval(result);if(Math.Abs(value)<=Polynomial.TOLERANCE){break;}if(value*minValue<0){max=result;maxValue=value;}else{min=result;minValue=value;}}}return result;}
        //Polynomial toString(){var coefs=new List<double>();var signs=new List<string>();for(var i=this.coefs.Count-1;i>=0;i--){var value=this.coefs[i];if(value!=0){var sign=(value<0)?" - ":" + ";value=Math.Abs(value);if(i>0)if(value==1)value="x";else value+="x";if(i>1)value+="^"+i;signs.Add(sign);coefs.Add(value);}}signs[0]=(signs[0]==" + ")?"":"-";var result="";for(var i=0;i<coefs.Count;i++)result+=signs[i]+coefs[i];return result;}
        int getDegree()
        {
            return this.coefs.Count - 1;
        }
        //Polynomial getDerivative(){var derivative=new Polynomial();for(var i=1;i<this.coefs.Count;i++){derivative.coefs.Add(i*this.coefs[i]);}return derivative;}
        public List<double> getRoots()
        {
            List<double> result; this.simplify();
            switch (this.getDegree())
            {
                case 0:
                    result = new List<double>();
                    break;
                case 1:
                    result = this.getLinearRoot();
                    break;
                case 2:
                    result = this.getQuadraticRoots();
                    break;
                case 3:
                    result = this.getCubicRoots();
                    break;
                case 4:
                    result = this.getQuarticRoots();
                    break;
                default:
                    result = new List<double>();
                    break;
            }
            return result;
        }
        //List<double> getRootsInInterval(double min,double max){var roots=new List<double>();double root;if(this.getDegree()==1){root=this.bisection(min,max);if(root!=null)roots.Add(root);}else{var deriv=this.getDerivative();var droots=deriv.getRootsInInterval(min,max);if(droots.Count>0){root=this.bisection(min,droots[0]);if(root!=null)roots.Add(root);for(i=0;i<=droots.Count-2;i++){root=this.bisection(droots[i],droots[i+1]);if(root!=null)roots.Add(root);}root=this.bisection(droots[droots.Length-1],max);if(root!=null)roots.Add(root);}else{root=this.bisection(min,max);if(root!=null)roots.Add(root);}}return roots;}
        List<double> getLinearRoot()
        {
            var result = new List<double>();
            var a = this.coefs[1];
            if (a != 0)
                result.Add(-this.coefs[0] / a);
            return result;
        }
        List<double> getQuadraticRoots()
        {
            var results = new List<double>();
            if (this.getDegree() == 2)
            {
                var a = this.coefs[2];
                var b = this.coefs[1] / a;
                var c = this.coefs[0] / a;
                var d = b * b - 4 * c;
                if (d > 0)
                {
                    var e = Math.Sqrt(d);
                    results.Add(0.5 * (-b + e));
                    results.Add(0.5 * (-b - e));
                }
                else if (d == 0)
                {
                    results.Add(0.5 * -b);
                }
            }
            return results;
        }
        List<double> getCubicRoots()
        {
            var results = new List<double>();
            if (this.getDegree() == 3)
            {
                var c3 = this.coefs[3];
                var c2 = this.coefs[2] / c3;
                var c1 = this.coefs[1] / c3;
                var c0 = this.coefs[0] / c3;
                var a = (3 * c1 - c2 * c2) / 3;
                var b = (2 * c2 * c2 * c2 - 9 * c1 * c2 + 27 * c0) / 27;
                var offset = c2 / 3;
                var discrim = b * b / 4 + a * a * a / 27;
                var halfB = b / 2;
                if (Math.Abs(discrim) <= Polynomial.TOLERANCE)
                    discrim = 0;
                if (discrim > 0)
                {
                    var e = Math.Sqrt(discrim);
                    double tmp;
                    double root;
                    tmp = -halfB + e;
                    if (tmp >= 0)
                        root = Math.Pow(tmp, 1 / 3);
                    else
                        root = -Math.Pow(-tmp, 1 / 3);
                    tmp = -halfB - e;
                    if (tmp >= 0)
                        root += Math.Pow(tmp, 1 / 3);
                    else root -= Math.Pow(-tmp, 1 / 3);
                    results.Add(root - offset);
                }
                else if (discrim < 0)
                {
                    var distance = Math.Sqrt(-a / 3);
                    var angle = Math.Atan2(Math.Sqrt(-discrim), -halfB) / 3;
                    var cos = Math.Cos(angle);
                    var sin = Math.Sin(angle);
                    var sqrt3 = Math.Sqrt(3);
                    results.Add(2 * distance * cos - offset);
                    results.Add(-distance * (cos + sqrt3 * sin) - offset);
                    results.Add(-distance * (cos - sqrt3 * sin) - offset);
                }
                else
                {
                    double tmp;
                    if (halfB >= 0)
                        tmp = -Math.Pow(halfB, 1 / 3);
                    else
                        tmp = Math.Pow(-halfB, 1 / 3);
                    results.Add(2 * tmp - offset);
                    results.Add(-tmp - offset);
                }
            }
            return results;
        }
        List<double> getQuarticRoots()
        {
            var results = new List<double>(); if (this.getDegree() == 4)
            {
                var c4 = this.coefs[4]; var c3 = this.coefs[3] / c4;
                var c2 = this.coefs[2] / c4; var c1 = this.coefs[1] / c4; var c0 = this.coefs[0] / c4;
                var resolveRoots = new Polynomial(1, -c2, c3 * c1 - 4 * c0, -c3 * c3 * c0 + 4 * c2 * c0 - c1 * c1).getCubicRoots();
                var y = resolveRoots[0];
                var discrim = c3 * c3 / 4 - c2 + y;
                if (Math.Abs(discrim) <= Polynomial.TOLERANCE)
                    discrim = 0;
                if (discrim > 0)
                {
                    var e = Math.Sqrt(discrim);
                    var t1 = 3 * c3 * c3 / 4 - e * e - 2 * c2; var t2 = (4 * c3 * c2 - 8 * c1 - c3 * c3 * c3) / (4 * e);
                    var plus = t1 + t2;
                    var minus = t1 - t2;
                    if (Math.Abs(plus) <= Polynomial.TOLERANCE)
                        plus = 0;
                    if (Math.Abs(minus) <= Polynomial.TOLERANCE)
                        minus = 0;
                    if (plus >= 0)
                    {
                        var f = Math.Sqrt(plus);
                        results.Add(-c3 / 4 + (e + f) / 2);
                        results.Add(-c3 / 4 + (e - f) / 2);
                    }
                    if (minus >= 0)
                    {
                        var f = Math.Sqrt(minus);
                        results.Add(-c3 / 4 + (f - e) / 2); results.Add(-c3 / 4 - (f + e) / 2);
                    }
                }
                else if (discrim < 0) { }
                else
                {
                    var t2 = y * y - 4 * c0;
                    if (t2 >= -Polynomial.TOLERANCE)
                    {
                        if (t2 < 0)
                            t2 = 0;
                        t2 = 2 * Math.Sqrt(t2);
                        var t1 = 3 * c3 * c3 / 4 - 2 * c2;
                        if (t1 + t2 >= Polynomial.TOLERANCE)
                        {
                            var d = Math.Sqrt(t1 + t2);
                            results.Add(-c3 / 4 + d / 2);
                            results.Add(-c3 / 4 - d / 2);
                        }
                        if (t1 - t2 >= Polynomial.TOLERANCE)
                        {
                            var d = Math.Sqrt(t1 - t2);
                            results.Add(-c3 / 4 + d / 2);
                            results.Add(-c3 / 4 - d / 2);
                        }
                    }
                }
            }
            return results;
        }
    }
}
