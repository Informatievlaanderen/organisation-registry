namespace OrganisationRegistry.Infrastructure.Infrastructure;

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

internal class PrivateReflectionDynamicObject : DynamicObject
{
    public object RealObject { get; set; } = null!;
    private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    internal static object? WrapObjectIfNeeded(object? o)
    {
        // Don't wrap primitive types, which don't have many interesting internal APIs
        if (o == null || o.GetType().GetTypeInfo().IsPrimitive || o is string)
            return o;

        return new PrivateReflectionDynamicObject { RealObject = o };
    }

    // Called when a method is called
    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
    {
        result = InvokeMemberOnType(RealObject.GetType(), RealObject, binder.Name, args);

        // Wrap the sub object if necessary. This allows nested anonymous objects to work.
        result = WrapObjectIfNeeded(result);

        return true;
    }

    private static object? InvokeMemberOnType(Type type, object target, string name, object?[]? args)
    {
        args ??= Array.Empty<object>();

        var argtypes = new Type[args.Length];
        var typeInfo = type.GetTypeInfo();

        for (var i = 0; i < args.Length; i++)
            argtypes[i] = args[i]!.GetType();

        while (true)
        {
            //var member = type.GetMethod(name, bindingFlags, null, argtypes, null);
            var member = type.GetMethods(bindingFlags).FirstOrDefault(x => CheckArgs(x.GetParameters(), argtypes));

            if (member != null) return member.Invoke(target, args);
            if (typeInfo.BaseType == null) return null;
            type = typeInfo.BaseType;
            typeInfo = type.GetTypeInfo();
        }
    }

    private static bool CheckArgs(IReadOnlyCollection<ParameterInfo> methodTypes, Type[] compareTypes)
    {
        if (methodTypes.Count != compareTypes.Length)
            return false;

        if (methodTypes.Count == 0 && compareTypes.Length == 0)
            return true;

        var firstTypesOrdered = methodTypes.Select(x => x.ParameterType).OrderBy(x => x.Name).ToArray();
        var secondTypesOrdered = compareTypes.OrderBy(x => x.Name).ToArray();

        return !firstTypesOrdered.Where((t, i) => t.FullName != secondTypesOrdered[i].FullName).Any();
    }
}
