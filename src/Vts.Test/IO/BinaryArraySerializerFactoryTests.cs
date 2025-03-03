using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Vts.IO;

namespace Vts.Test.IO;

[TestFixture]
public class BinaryArraySerializerFactoryTests
{
    private double[] _doubleArray;
    private double[,] _doubleArray2d;
    private double[,,] _doubleArray3d;
    private double[,,,] _doubleArray4d;
    private double[,,,,] _doubleArray5d;
    private double[,,,,,] _doubleArray6d;
    private Complex[] _complexArray;
    private Complex[,] _complexArray2d;
    private Complex[,,] _complexArray3d;
    private Complex[,,,] _complexArray4d;
    private Complex[,,,,] _complexArray5d;
    private Complex[,,,,,] _complexArray6d;

    [OneTimeSetUp]
    public void Setup()
    {
        _doubleArray = new[] { 2.8, 3.5 };
        _doubleArray2d = new[,] { { 1.5, 6.2 }, { 5.1, 6.7 }, { 4.0, 3.2 } };
        _doubleArray3d = new double[,,] { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } }, { { 9, 10 }, { 11, 12 } } };
        _doubleArray4d = new double[,,,]
        {
            {
                {
                    {1, 0}, {2, 1}
                },
                {
                    {3, 5}, {4, 2}
                }
            },
            {
                {
                    {2, 6}, {2, 2}
                },
                {
                    {0, 0}, {9, 1}
                }
            },
            {
                {
                    {6, 4}, {3, 1}
                },
                {
                    {3, 0}, {7, 1}
                }
            }
        };
        _doubleArray5d = new double[,,,,]
        {
            {
                {
                    {
                        {3, 5}, {4, 2}
                    },
                    {
                        {0, 1}, {2, 3}
                    }
                },
                {
                    {
                        {2, 6}, {2, 2}
                    },
                    {
                        {0, 0}, {9, 1}
                    }
                }
            },
            {
                {
                    {
                        {3, 0}, {7, 1}
                    },
                    {
                        {6, 4}, {3, 1}
                    }
                },
                {
                    {
                        {6, 4}, {3, 1}
                    },
                    {
                        {3, 0}, {7, 1}
                    }
                }
            },
            {
                {
                    {
                        {3, 0}, {7, 1}
                    },
                    {
                        {2, 6}, {2, 2}
                    }
                },
                {
                    {
                        {2, 6}, {2, 2}
                    },
                    {
                        {3, 0}, {7, 1}
                    }
                }
            }
        };
        _doubleArray6d = new double[,,,,,]
        {
            {
                {
                    {
                        {
                            {3, 5}, {4, 2}
                        },
                        {
                            {3, 5}, {4, 2}
                        }
                    },
                    {
                        {
                            {3, 5}, {4, 2}
                        },
                        {
                            {3, 5}, {4, 2}
                        }
                    }
                },
                {
                    {
                        {
                            {3, 5}, {4, 2}
                        },
                        {
                            {3, 5}, {4, 2}
                        }
                    },
                    {
                        {
                            {3, 5}, {4, 2}
                        },
                        {
                            {3, 5}, {4, 2}
                        }
                    }
                }
            },
            {
                {
                    {
                        {
                            {3, 5}, {4, 2}
                        },
                        {
                            {3, 5}, {4, 2}
                        }
                    },
                    {
                        {
                            {3, 5}, {4, 2}
                        },
                        {
                            {3, 5}, {4, 2}
                        }
                    }
                },
                {
                    {
                        {
                            {3, 5}, {4, 2}
                        },
                        {
                            {3, 5}, {4, 2}
                        }
                    },
                    {
                        {
                            {3, 5}, {4, 2}
                        },
                        {
                            {3, 5}, {4, 2}
                        }
                    }
                }
            }
        };
        // Complex Arrays
        _complexArray = new Complex[] { 2.8, 3.5 };
        _complexArray2d = new Complex[,] { { 1.5, 6.2 }, { 5.1, 6.7 }, { 4.0, 3.2 } };
        _complexArray3d = new Complex[,,] { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } }, { { 9, 10 }, { 11, 12 } } };
        _complexArray4d = new Complex[,,,]
        {
            {
                {
                    {1, 0}, {2, 1}
                },
                {
                    {3, 5}, {4, 2}
                }
            },
            {
                {
                    {2, 6}, {2, 2}
                },
                {
                    {0, 0}, {9, 1}
                }
            },
            {
                {
                    {6, 4}, {3, 1}
                },
                {
                    {3, 0}, {7, 1}
                }
            }
        };
        _complexArray5d = new Complex[,,,,]
        {
            {
                {
                    {
                        {3, 5}, {4, 2}
                    },
                    {
                        {0, 1}, {2, 3}
                    }
                },
                {
                    {
                        {2, 6}, {2, 2}
                    },
                    {
                        {0, 0}, {9, 1}
                    }
                }
            },
            {
                {
                    {
                        {3, 0}, {7, 1}
                    },
                    {
                        {6, 4}, {3, 1}
                    }
                },
                {
                    {
                        {6, 4}, {3, 1}
                    },
                    {
                        {3, 0}, {7, 1}
                    }
                }
            },
            {
                {
                    {
                        {3, 0}, {7, 1}
                    },
                    {
                        {2, 6}, {2, 2}
                    }
                },
                {
                    {
                        {2, 6}, {2, 2}
                    },
                    {
                        {3, 0}, {7, 1}
                    }
                }
            }
        };
        _complexArray6d = new Complex[,,,,,]
        {
            {
                {
                    {
                        {
                            {3, 5}, {4, 2}
                        },
                        {
                            {3, 5}, {4, 2}
                        }
                    },
                    {
                        {
                            {3, 5}, {4, 2}
                        },
                        {
                            {3, 5}, {4, 2}
                        }
                    }
                },
                {
                    {
                        {
                            {3, 5}, {4, 2}
                        },
                        {
                            {3, 5}, {4, 2}
                        }
                    },
                    {
                        {
                            {3, 5}, {4, 2}
                        },
                        {
                            {3, 5}, {4, 2}
                        }
                    }
                }
            },
            {
                {
                    {
                        {
                            {3, 5}, {4, 2}
                        },
                        {
                            {3, 5}, {4, 2}
                        }
                    },
                    {
                        {
                            {3, 5}, {4, 2}
                        },
                        {
                            {3, 5}, {4, 2}
                        }
                    }
                },
                {
                    {
                        {
                            {3, 5}, {4, 2}
                        },
                        {
                            {3, 5}, {4, 2}
                        }
                    },
                    {
                        {
                            {3, 5}, {4, 2}
                        },
                        {
                            {3, 5}, {4, 2}
                        }
                    }
                }
            }
        };
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Test_2D_real_serializer(bool tallySecondMoment)
    {
        var mean = new double[5, 4];
        var secondMoment = new double[5, 4];
        var allSerializers = new List<BinaryArraySerializer>
        {
            BinaryArraySerializerFactory.GetSerializer(
                mean, "Mean", ""),
            tallySecondMoment ? BinaryArraySerializerFactory.GetSerializer(
                secondMoment, "SecondMoment", "_2") : null
        };
        var serializers = allSerializers.Where(s => s is not null).ToArray();
        Assert.That(serializers.Length, Is.EqualTo(tallySecondMoment ? 2 : 1));
        Assert.That(serializers[0].Name, Is.EqualTo("Mean"));
        Assert.That(serializers[0].FileTag, Is.EqualTo(""));
        Assert.That(serializers[0].ReadData, Is.InstanceOf<Action<BinaryReader>>());
        Assert.That(serializers[0].WriteData, Is.InstanceOf<Action<BinaryWriter>>());
        if (!tallySecondMoment) return;
        Assert.That(serializers[1].Name, Is.EqualTo("SecondMoment"));
        Assert.That(serializers[1].FileTag, Is.EqualTo("_2"));
        Assert.That(serializers[1].ReadData, Is.InstanceOf<Action<BinaryReader>>());
        Assert.That(serializers[1].WriteData, Is.InstanceOf<Action<BinaryWriter>>());
    }

    [Test]
    public void Test_not_implemented_real_serializers()
    {
        var mean7D = new double[5, 4, 5, 7, 6, 2, 4];
        Assert.Throws<NotImplementedException>(() => BinaryArraySerializerFactory.GetSerializer(
            mean7D, "Mean7d", "FileTag7"));
    }

    [Test]
    public void Test_not_implemented_complex_serializers()
    {
        var mean7D = new Complex[5, 4, 5, 7, 6, 2, 4];
        Assert.Throws<NotImplementedException>(() => BinaryArraySerializerFactory.GetSerializer(
            mean7D, "Mean7d", "FileTag7"));
    }

    [Test]
    public void Test_all_dimension_real_serializers()
    {
        var serializers = new[]
        {
            BinaryArraySerializerFactory.GetSerializer(
                _doubleArray, "Mean1d", "FileTag1"),
            BinaryArraySerializerFactory.GetSerializer(
                _doubleArray2d, "Mean2d", "FileTag2"),
            BinaryArraySerializerFactory.GetSerializer(
                _doubleArray3d, "Mean3d", "FileTag3"),
            BinaryArraySerializerFactory.GetSerializer(
                _doubleArray4d, "Mean4d", "FileTag4"),
            BinaryArraySerializerFactory.GetSerializer(
                _doubleArray5d, "Mean5d", "FileTag5"),
            BinaryArraySerializerFactory.GetSerializer(
                _doubleArray6d, "Mean6d", "FileTag6")
        };
        Assert.That(serializers.Length, Is.EqualTo(6));
        for (var i = 0; i < serializers.Length; i++)
        {
            Assert.That(serializers[i].Name, Is.EqualTo($"Mean{i + 1}d"));
            Assert.That(serializers[i].FileTag, Is.EqualTo($"FileTag{i + 1}"));
            Assert.That(serializers[i].ReadData, Is.InstanceOf<Action<BinaryReader>>());
            Assert.That(serializers[i].WriteData, Is.InstanceOf<Action<BinaryWriter>>());
        }
        VerifyBinaryReadWriteActions(serializers);
    }

    [Test]
    public void Test_all_dimension_complex_serializers()
    {
        var serializers = new[]
        {
            BinaryArraySerializerFactory.GetSerializer(
                _complexArray, "Mean1d", "FileTag1"),
            BinaryArraySerializerFactory.GetSerializer(
                _complexArray2d, "Mean2d", "FileTag2"),
            BinaryArraySerializerFactory.GetSerializer(
                _complexArray3d, "Mean3d", "FileTag3"),
            BinaryArraySerializerFactory.GetSerializer(
                _complexArray4d, "Mean4d", "FileTag4"),
            BinaryArraySerializerFactory.GetSerializer(
                _complexArray5d, "Mean5d", "FileTag5"),
            BinaryArraySerializerFactory.GetSerializer(
                _complexArray6d, "Mean6d", "FileTag6")
        };
        Assert.That(serializers.Length, Is.EqualTo(6));
        for (var i = 0; i < serializers.Length; i++)
        {
            Assert.That(serializers[i].Name, Is.EqualTo($"Mean{i + 1}d"));
            Assert.That(serializers[i].FileTag, Is.EqualTo($"FileTag{i + 1}"));
            Assert.That(serializers[i].ReadData, Is.InstanceOf<Action<BinaryReader>>());
            Assert.That(serializers[i].WriteData, Is.InstanceOf<Action<BinaryWriter>>());
        }
        VerifyBinaryReadWriteActions(serializers);
    }

    [Test]
    public void Test_GetReadDataArrayAction_throws_not_implemented_exception()
    {
        var mean7D = new Complex[5, 4, 5, 7, 6, 2, 4];
        Assert.Throws<NotImplementedException>(() => BinaryArraySerializerFactory.GetReadDataArrayAction(mean7D));
    }

    [Test]
    public void Test_GetWriteDataArrayAction_throws_not_implemented_exception()
    {
        var mean7D = new Complex[5, 4, 5, 7, 6, 2, 4];
        Assert.Throws<NotImplementedException>(() => BinaryArraySerializerFactory.GetWriteDataArrayAction(mean7D));
    }

    private static void VerifyBinaryReadWriteActions(IEnumerable<BinaryArraySerializer> binaryArraySerializers)
    {
        foreach (var binaryArraySerializer in binaryArraySerializers)
        {
            if (binaryArraySerializer == null)
                continue;

            using var stream = new MemoryStream();
            using var bw = new BinaryWriter(stream);
            binaryArraySerializer.WriteData(bw);
            stream.Position = 0;
            using var br = new BinaryReader(stream);
            binaryArraySerializer.ReadData(br);
        }
    }
}