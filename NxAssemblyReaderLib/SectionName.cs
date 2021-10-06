using NLog;
using NXOpen;
using NXOpen.Assemblies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NxAssemblyReaderLib
{
    public class SectionName
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        public static string Find<T>(T part) where T : NXObject
        {
            try
            {
                return part.GetUserAttributeAsString("DB_PART_NAME", NXObject.AttributeType.String, -1);
            }
            catch (NXException ex)
            {
                _logger.Warn(ex, "DB name not found");
            }
            return part.Name;
        }
        public static string Find(BasePart part)
        {
            try
            {
                return part.GetUserAttributeAsString("DB_PART_NAME", NXObject.AttributeType.String, -1);
            }
            catch (NXException ex)
            {
                _logger.Warn(ex, "DB name not found");
            }
            return part.Name;
        }
        public static string Find(Component part)
        {
            try
            {
                return part.GetUserAttributeAsString("DB_PART_NAME", NXObject.AttributeType.String, -1);
            }
            catch (NXException ex)
            {
                _logger.Warn(ex, "DB name not found");
            }
            return part.Name;
        }
    }
}
