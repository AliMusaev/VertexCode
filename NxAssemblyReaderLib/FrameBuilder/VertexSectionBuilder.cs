using NLog;
using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VertexCodeMakerDomain;

namespace NxAssemblyReaderLib.FrameBuilder
{
    class VertexSectionBuilder
    {
        private ErrorMessageStore _erStore;
        private TemplateSection _currentTemplate;
        private string _currentComponentName;
        private Logger _logger = LogManager.GetCurrentClassLogger();
        public VertexSectionBuilder()
        {
            _erStore = ErrorMessageStore.GetStore();
        }
        public VertexSection BuildSectionBy(TemplateSection template, string componentName, Point3d initPoint, Matrix3x3 matrix, SecondAxis axis)
        {
            _currentTemplate = template;
            _currentComponentName = componentName; 
            double[] angles = DefineAngles(matrix, axis);
            Point2d[] points = DefineExtremePoints(initPoint, matrix, template.Width, template.Height, axis, angles[2], angles[3]);
            ShelvsDirection direction = DefineShelvDirection(angles[2], angles[3]);

            return new VertexSection(template, points[0], points[1], angles, direction);
        }
        private double[] DefineAngles(Matrix3x3 matrix, SecondAxis axis)
        {
            double[] retVal = new double[4];
            int digits = 3;
            retVal[0] = Math.Round(matrix.Xx, digits);
            retVal[1] = axis == SecondAxis.Y ? Math.Round(matrix.Xy, digits) : axis == SecondAxis.Z ? Math.Round(matrix.Xz, digits) : throw new Exception();
            retVal[2] = Math.Round(matrix.Zx, digits);
            retVal[3] = axis == SecondAxis.Y ? Math.Round(matrix.Zy, digits) : axis == SecondAxis.Z ? Math.Round(matrix.Zz, digits) : throw new Exception();
            return retVal;
        }
        private Point2d[] DefineExtremePoints(Point3d point, Matrix3x3 directionMatrix, double width, double height, SecondAxis axis, double cosY, double sinY)
        {
            int digits = 3;
            Point2d[] retVal = new Point2d[2]; 
            var initPoint = point.Round(digits);
            Point3d endPoint = new Point3d(initPoint.X + (width * directionMatrix.Xx),
                                           initPoint.Y + (width * directionMatrix.Xy),
                                           initPoint.Z + (width * directionMatrix.Xz)).Round(digits);
            var cosH = Math.Round(height / 2 * cosY, digits);
            var sinH = Math.Round(height / 2 * sinY, digits);


            retVal[0] = new Point2d(initPoint.X + cosH, axis == SecondAxis.Y ? initPoint.Y + sinH :
                                                         axis == SecondAxis.Z ? initPoint.Z + sinH : throw new Exception());
            retVal[1] = new Point2d(endPoint.X + cosH, axis == SecondAxis.Y ? endPoint.Y + sinH :
                                                      axis == SecondAxis.Z ? endPoint.Z + sinH : throw new Exception());
            return retVal;
        }
        private ShelvsDirection DefineShelvDirection(double cosY, double sinY)
        {
            double mid = 0.707;
            if (cosY <= mid && cosY >= -mid && sinY >= mid && sinY <= 1)
            {
                return ShelvsDirection.U;
            }
            else if (cosY <= -mid && cosY >= -1 && sinY <= mid && sinY >= -mid)
            {
                return ShelvsDirection.L;
            }
            else if (cosY >= -mid && cosY <= mid && sinY <= -mid && sinY >= -1)
            {
                return ShelvsDirection.D;
            }
            else if (cosY >= mid && cosY <= 1 && sinY >= -mid && sinY <= mid)
            {
                return ShelvsDirection.R;
            }
            else
            {
                //MessageBox.Show($"Cant define section Direction in component {_currentComponentName}. Will be set default direction: U");
                _erStore.AddMessage(new ErrorMessage(_currentComponentName, "Cant define section Direction"));
                _logger.Warn($"Cant define section Direction in component {_currentComponentName}. Will be set default direction: U");
                return ShelvsDirection.U;
            }
        }
    }
}
