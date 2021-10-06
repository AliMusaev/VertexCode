using NLog;
using NXOpen;
using NXOpen.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NxAssemblyReaderLib.BaseBuilder
{
    class FeaturesHandler
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();
        public T CastNxObjectTo<T>(NXObject obj) where T : NXObject
        {
            try
            {
                return (T)obj;
            }
            catch (Exception)
            {

                throw new Exception($"Cant cast \"{obj.Name}\" with type {obj.GetType().Name} to {typeof(T).Name}");
            }
        }
        public List<T> CastFeaturesTo<T>(List<Feature> features) where T : NXObject
        {
            try
            {
                return features.Select(x => CastNxObjectTo<T>(x)).ToList();

            }
            catch (Exception)
            {

                throw;
            }
            
        }
        public NXObject[] GetGeometryFrom(Feature feature, int exceptedAmount = 0)
        {
            var parents = feature.GetParents();
            if (parents.Length != 1)
            {
                throw new Exception($"feature \"{feature.Name}\" dont contain sketch or contain more then 1");
            }
            var geoms = ((SketchFeature)parents[0]).Sketch.GetAllGeometry();
            if (exceptedAmount == 0)
            {
                if (geoms.Length < 1) throw new Exception($"sketch of feature \"{feature.Name}\" dont contain elements");
                else return geoms;
            }
            else
            {
                if (geoms.Length != exceptedAmount) throw new Exception($"sketch of feature \"{feature.Name}\" containt elements is not equal {exceptedAmount}");
                else return geoms;
            }
        }
        public List<Feature> FindIn<T>(BasePart part) where T : class
        {
            try
            {
                return part.Features.ToArray().Where(x => x.GetType().Name == typeof(T).Name).ToList();
            }
            catch (Exception)
            {

                throw new Exception($"part \"{part.Name}\" dont contain feature by type {typeof(T).Name}");
            }

        }
        public List<Feature> FindIn<T>(BasePart part, string name) where T : class
        {
            try
            {
                return part.Features.ToArray().Where(x => x.GetType().Name == typeof(T).Name && x.FeatureType == name).ToList();

            }
            catch (Exception)
            {

                throw new Exception($"part \"{part.Name}\" dont contain feature by type {typeof(T).Name}. Control name - {name}");
            }

        }
       
    }
}
