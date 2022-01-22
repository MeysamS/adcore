using ADCore.Mapper.Models;

namespace ADCore.Mapper.Services
{
    public interface IMapperService 
    {

        /// <summary>
        /// Map Json Data in Resource to Specific Model By Config in Resource 
        ///first try with Expression Tree, if cant, try with Reflection
        ///again if cant map some property, will map other property as far as possible
        /// </summary>
        /// <param name="resource">resource contain data and config dictionary</param>
        /// <param name="target"></param>
        /// <returns>return an bool that indicate Mapping Opration is Ok</returns>
        public bool TryMap<T>(Resource resource, out object target);

        /// <summary>
        /// Map Json Data in Resource to Specific Model By Config in Resource 
        /// by Expression Tree
        /// </summary>
        /// <param name="resource">resource contain data and config dictionary</param>
        /// <returns>return an object </returns>
        public object Map<T>(Resource resource);
    }




}
