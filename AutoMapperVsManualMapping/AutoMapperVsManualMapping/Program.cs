#define MappingByAutoMapper

using AutoMapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AutoMapperVsManualMapping;

static class Program
{
    static IMapper mapper;

    private static void Main(string[] args)
    {
        for (int i = 0; i < 20; i++)
        {
            var toMap = Init(10000);

#if MappingByAutoMapper
            InitAutoMapper();
#endif

            var stopwatch = Stopwatch.StartNew();

#if MappingByAutoMapper
            var a = MappingByAutoMapper(toMap);
#else
            var a = ManualMapping(toMap);
#endif

            stopwatch.Stop();

            Console.WriteLine($"Result: { stopwatch.ElapsedMilliseconds }");
        }
    }


    private static IEnumerable<SomeClassB> ManualMapping(IEnumerable<SomeClassA> toMap)
    {
        return toMap.Select(
            item => new SomeClassB
            {
                Id               = item.Id,
                IntValue1        = item.IntValue1,
                IntValue2        = item.IntValue2,
                IntValue3        = item.IntValue3,
                StringValue1     = item.StringValue1,
                StringValue2     = item.StringValue2,
                StringValue3     = item.StringValue3,
                RandomValues     = item.RandomValues.ToList(),
                SomeNestedObject = new SomeNestedClassB()
                {
                    IntValue    = item.SomeNestedObject.IntValue,
                    StringValue = item.SomeNestedObject.StringValue
                },
                SomeList         = item.SomeList.Select(
                    i => new SomeNestedClassB()
                    {
                        IntValue    = i.IntValue,
                        StringValue = i.StringValue
                    })
            });
    }

    private static IEnumerable<SomeClassB> MappingByAutoMapper(IEnumerable<SomeClassA> toMap)
        => mapper.Map<List<SomeClassB>>(toMap);

    private static void InitAutoMapper()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<SomeClassA, SomeClassB>()
                    .ForMember(d => d.Id,               o => o.MapFrom(x => x.Id))

                    .ForMember(d => d.IntValue1,        o => o.MapFrom(x => x.IntValue1))
                    .ForMember(d => d.IntValue2,        o => o.MapFrom(x => x.IntValue2))
                    .ForMember(d => d.IntValue3,        o => o.MapFrom(x => x.IntValue3))

                    .ForMember(d => d.RandomValues,     o => o.MapFrom(x => x.RandomValues))
                    .ForMember(d => d.SomeList,         o => o.MapFrom(x => x.SomeList))
                    .ForMember(d => d.SomeNestedObject, o => o.MapFrom(x => x.SomeNestedObject))

                    .ForMember(d => d.StringValue1,     o => o.MapFrom(x => x.StringValue1))
                    .ForMember(d => d.StringValue2,     o => o.MapFrom(x => x.StringValue2))
                    .ForMember(d => d.StringValue3,     o => o.MapFrom(x => x.StringValue3));


                cfg.CreateMap<SomeNestedClassA, SomeNestedClassB>()
                    .ForMember(d => d.IntValue,    o => o.MapFrom(x => x.IntValue))
                    .ForMember(d => d.StringValue, o => o.MapFrom(x => x.StringValue));
            });

        mapper = config.CreateMapper();
    }

    private static IEnumerable<SomeClassA> Init(int count)
    {
        var random = new Random();
        var list = new List<SomeClassA>(count);

        for (int i = 0; i < count; i++)
        {
            var nestedList = new List<SomeNestedClassA>();

            for (int j = 0; j < 30; ++j)
            {
                nestedList.Add(
                    new SomeNestedClassA
                    {
                        IntValue    = random.Next(int.MinValue, int.MaxValue),
                        StringValue = GetRandomString(random.Next(10, 20))
                    });
            }

            var item = new SomeClassA
            {
                Id               = Guid.NewGuid(),
                IntValue1        = random.Next(int.MinValue, int.MaxValue),
                IntValue2        = random.Next(int.MinValue, int.MaxValue),
                IntValue3        = random.Next(int.MinValue, int.MaxValue),
                RandomValues     = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                StringValue1     = GetRandomString(random.Next(10, 20)),
                StringValue2     = GetRandomString(random.Next(10, 20)),
                StringValue3     = GetRandomString(random.Next(10, 20)),
                SomeList         = nestedList,
                SomeNestedObject = new SomeNestedClassA()
                {
                    IntValue    = random.Next(int.MinValue, int.MaxValue),
                    StringValue = GetRandomString(random.Next(10, 20))
                }
            };

            list.Add(item);
        }

        return list;
    }

    private static string GetRandomString(int length)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var str = string.Empty;
        var random = new Random();

        for (int i = 0; i < length; i++)
        {
            str += chars[random.Next(chars.Length)];
        }

        return str;
    }
}

#region Models

public sealed class SomeClassA
{
    public Guid Id { get; set; }

    public int IntValue1 { get; set; }
    public int IntValue2 { get; set; }
    public int IntValue3 { get; set; }

    public string StringValue1 { get; set; }
    public string StringValue2 { get; set; }
    public string StringValue3 { get; set; }

    public SomeNestedClassA SomeNestedObject { get; set; }
    public IEnumerable<int> RandomValues { get; set; }
    public IEnumerable<SomeNestedClassA> SomeList { get; set; }
}

public sealed class SomeNestedClassA
{
    public int IntValue { get; set; }
    public string StringValue { get; set; }
}



public sealed class SomeClassB
{
    public Guid Id { get; set; }

    public int IntValue1 { get; set; }
    public int IntValue2 { get; set; }
    public int IntValue3 { get; set; }

    public string StringValue1 { get; set; }
    public string StringValue2 { get; set; }
    public string StringValue3 { get; set; }

    public SomeNestedClassB SomeNestedObject { get; set; }
    public IEnumerable<int> RandomValues { get; set; }
    public IEnumerable<SomeNestedClassB> SomeList { get; set; }
}

public sealed class SomeNestedClassB
{
    public int IntValue { get; set; }
    public string StringValue { get; set; }
}

#endregion
