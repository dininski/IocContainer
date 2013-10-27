﻿namespace VasContainer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Container
    {
        private IDictionary<Type, Type> dependecies;

        private ContainerOptions options;

        private const ContainerOptions defaultOptions = ContainerOptions.None;

        public Container()
            : this(defaultOptions)
        {
        }

        public Container(ContainerOptions options)
        {
            this.options = options;
            this.dependecies = new Dictionary<Type, Type>();
        }

        public void RegisterDependency(Type dependency, Type classToResolve)
        {
            this.dependecies.Add(dependency, classToResolve);
        }

        public T Resolve<T>() where T : class
        {
            var classType = typeof(T);

            var constructors = classType
                .GetConstructors()
                // create a smarter way of choosing a constructor
                // maybe using enum flags :-P
                .OrderByDescending(x => x.GetParameters().Count());

            if (!constructors.Any())
            {
                throw new ArgumentException("The class to be resolved does not have any public constructors!");
            }

            foreach (var constructor in constructors)
            {
                // get the constructor parameters
                var parameters = constructor.GetParameters();

                // if the constructor has no parameters - instantiate the object and return it;
                if (parameters.Length == 0)
                {
                    var result = Activator.CreateInstance<T>();
                    return result;
                }
                else
                {
                    var parameterObjects = new List<object>();

                    foreach (var parameter in parameters)
                    {
                        var parameterType = parameter.ParameterType;

                        if (this.options.HasFlag(ContainerOptions.UseDefaultValue))
                        {
                            if (parameter.HasDefaultValue)
                            {
                                // using default value
                                var res = Convert.ChangeType(parameter.DefaultValue, parameterType);
                                parameterObjects.Add(res);
                                continue;
                            }
                        }

                        if (parameterType.IsAbstract || parameterType.IsInterface)
                        {
                            //TODO: review and check if the binding flags are persisting along the way
                            var concreteObjectType = this.dependecies[parameterType];
                            var method =
                                typeof(Container)
                                    .GetMethod("Resolve")
                                    .MakeGenericMethod(concreteObjectType);
                            var obj = method.Invoke(this, null);
                            parameterObjects.Add(obj);
                        }
                    }

                    var createdObject = (T)Activator.CreateInstance(classType, parameterObjects.ToArray());
                    return createdObject;
                }
            }

            throw new NotImplementedException();
        }
    }
}
