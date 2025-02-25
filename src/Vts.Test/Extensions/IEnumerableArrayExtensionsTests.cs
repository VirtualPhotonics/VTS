using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Numerics;
using Vts.Extensions;

namespace Vts.Test.Extensions
{
    [TestFixture]
    internal class IEnumerableArrayExtensionsTests
    {
        private Complex[] _complexArray;
        private double[,] _doubleArray;
        private ushort[,,] _uShortArray;
        private ushort[,,,] _uShortArray4D;
        private int[,,,,] _intArray;
        private int[,,,,,] _intArray6D;
        private int[,,,,,,] _intArray7D;
        private IEnumerable<Complex> _complexEnumerable;
        private IEnumerable<double> _doubleEnumerable;
        private IEnumerable<ushort> _uShortEnumerable;
        private IEnumerable<ushort> _uShortEnumerable4D;
        private IEnumerable<int> _intEnumerable;
        private IEnumerable<int> _intEnumerable6D;
        private IEnumerable<int> _intEnumerable7D;
        private int _length;

        [OneTimeSetUp]
        public void One_time_setup()
        {
            _complexArray = new Complex[2] { new Complex(4.0, 0.0), new Complex(1.3, 0.85) };
            _doubleArray = new double[3, 2] { { 1.5, 6.2 }, { 5.1, 6.7 }, { 4.0, 3.2 } };
            _uShortArray = new ushort[3, 2, 2] { { { 1, 2 }, { 3, 4 } }, { { 5, 6 }, { 7, 8 } }, { { 9, 10 }, { 11, 12 } } };
            _uShortArray4D = new ushort[3, 2, 2, 2]
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
            _intArray = new int[3, 2, 2, 2, 2]
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
            _intArray6D = new int[2, 2, 2, 2, 2, 2]
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
            _intArray7D = new int[2, 2, 2, 2, 2, 2, 2]
            {
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
                },
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
                }
            };
        }

        [SetUp]
        public void Setup()
        {
            _length = 0;
        }

        [Test]
        public void ToEnumerable_array_type_returns_enumerable()
        {
            Array array = new Array[2] { null, null };
            var result = array.ToEnumerable<int>();
            Assert.That(result, Is.InstanceOf<IEnumerable<int>>());
        }

        [Test]
        public void ToEnumerable_multi_array_type_returns_enumerable()
        {
            var array = new[] { new Array[1] , new Array[1] };
            var result = array.ToEnumerable<int>();
            Assert.That(result, Is.InstanceOf<IEnumerable<int>>());
        }

        [Test]
        public void ToEnumerable_array_returns_enumerable()
        {
            _complexEnumerable = _complexArray.ToEnumerable<Complex>();
            Assert.That(_complexEnumerable, Is.InstanceOf<IEnumerable<Complex>>());
            _complexEnumerable.ForEach((x, index) =>
            {
                Assert.That(x, Is.InstanceOf<Complex>());
                var complexArray = _complexArray;
                Assert.IsNotNull(complexArray);
                Assert.AreEqual(x.Real, complexArray[index].Real);
                Assert.AreEqual(x.Imaginary, complexArray[index].Imaginary);
                _length++;
            });
            Assert.AreEqual(_complexArray.Length, _length);
        }

        [Test]
        public void ToEnumerable_multi_array_2_returns_enumerable()
        {
            _doubleEnumerable = _doubleArray.ToEnumerable<double>();
            Assert.That(_doubleEnumerable, Is.InstanceOf<IEnumerable<double>>());
            _doubleEnumerable.ForEach(x =>
            {
                Assert.That(x, Is.InstanceOf<double>());
                _length++;
            });
            Assert.AreEqual(_doubleArray.Length, _length);
        }

        [Test]
        public void ToEnumerable_multi_array_3_returns_enumerable()
        {
            _uShortEnumerable = _uShortArray.ToEnumerable<ushort>();
            Assert.That(_uShortEnumerable, Is.InstanceOf<IEnumerable<ushort>>());
            _uShortEnumerable.ForEach(x =>
            {
                Assert.That(x, Is.InstanceOf<ushort>());
                _length++;
            });
            Assert.AreEqual(_uShortArray.Length, _length);
        }

        [Test]
        public void ToEnumerable_multi_array_4_returns_enumerable()
        {
            _uShortEnumerable4D = _uShortArray4D.ToEnumerable<ushort>();
            Assert.That(_uShortEnumerable4D, Is.InstanceOf<IEnumerable<ushort>>());
            _uShortEnumerable4D.ForEach(x =>
            {
                Assert.That(x, Is.InstanceOf<ushort>());
                _length++;
            });
            Assert.AreEqual(_uShortArray4D.Length, _length);
        }

        [Test]
        public void ToEnumerable_multi_array_5_returns_enumerable()
        {
            _intEnumerable = _intArray.ToEnumerable<int>();
            Assert.That(_intEnumerable, Is.InstanceOf<IEnumerable<int>>());
            _intEnumerable.ForEach(x =>
            {
                Assert.That(x, Is.InstanceOf<int>());
                _length++;
            });
            Assert.AreEqual(_intArray.Length, _length);
        }

        [Test]
        public void ToEnumerable_multi_array_6_returns_enumerable()
        {
            _intEnumerable6D = _intArray6D.ToEnumerable<int>();
            Assert.That(_intEnumerable6D, Is.InstanceOf<IEnumerable<int>>());
            _intEnumerable6D.ForEach(x =>
            {
                Assert.That(x, Is.InstanceOf<int>());
                _length++;
            });
            Assert.AreEqual(_intArray6D.Length, _length);
        }

        [Test]
        public void ToEnumerable_multi_array_7_returns_enumerable()
        {
            _intEnumerable7D = _intArray7D.ToEnumerable<int>();
            Assert.That(_intEnumerable7D, Is.InstanceOf<IEnumerable<int>>());
            _intEnumerable7D.ForEach(x =>
            {
                Assert.That(x, Is.InstanceOf<int>());
                _length++;
            });
            Assert.AreEqual(_intArray7D.Length, _length);
        }

        [Test]
        public void ToEnumerable2_multi_array_2_returns_enumerable()
        {
            var doubleEnumerable2D = _doubleArray.ToEnumerable2<double>();
            Assert.That(doubleEnumerable2D, Is.InstanceOf<IEnumerable<double>>());
            doubleEnumerable2D.ForEach(x =>
            {
                Assert.That(x, Is.InstanceOf<double>());
                _length++;
            });
            Assert.AreEqual(_doubleArray.Length, _length);
        }

        [Test]
        public void ToEnumerable2_multi_array_3_returns_enumerable()
        {
            var intEnumerable3D = _uShortArray.ToEnumerable2<ushort>();
            Assert.That(intEnumerable3D, Is.InstanceOf<IEnumerable<ushort>>());
            intEnumerable3D.ForEach(x =>
            {
                Assert.That(x, Is.InstanceOf<ushort>());
                _length++;
            });
            Assert.AreEqual(_uShortArray.Length, _length);
        }

        [Test]
        public void ToEnumerable2_multi_array_4_returns_enumerable()
        {
            var intEnumerable4D = _uShortArray4D.ToEnumerable2<ushort>();
            Assert.That(intEnumerable4D, Is.InstanceOf<IEnumerable<ushort>>());
            intEnumerable4D.ForEach(x =>
            {
                Assert.That(x, Is.InstanceOf<ushort>());
                _length++;
            });
            Assert.AreEqual(_uShortArray4D.Length, _length);
        }

        [Test]
        public void ToEnumerable2_multi_array_5_returns_enumerable()
        {
            var intEnumerable5D = _intArray.ToEnumerable2<int>();
            Assert.That(intEnumerable5D, Is.InstanceOf<IEnumerable<int>>());
            intEnumerable5D.ForEach(x =>
            {
                Assert.That(x, Is.InstanceOf<int>());
                _length++;
            });
            Assert.AreEqual(_intArray.Length, _length);
        }

        [Test]
        public void ToEnumerable2_multi_array_6_returns_enumerable()
        {
            var intEnumerable6D = _intArray6D.ToEnumerable2<int>();
            Assert.That(intEnumerable6D, Is.InstanceOf<IEnumerable<int>>());
            intEnumerable6D.ForEach(x =>
            {
                Assert.That(x, Is.InstanceOf<int>());
                _length++;
            });
            Assert.AreEqual(_intArray6D.Length, _length);
        }

        [Test]
        public void ToEnumerable2_multi_array_7_returns_enumerable()
        {
            var intEnumerable7D = _intArray7D.ToEnumerable2<int>();
            Assert.That(intEnumerable7D, Is.InstanceOf<IEnumerable<int>>());
            intEnumerable7D.ForEach(x =>
            {
                Assert.That(x, Is.InstanceOf<int>());
                _length++;
            });
            Assert.AreEqual(_intArray7D.Length, _length);
        }

        [Test]
        public void PopulateFromEnumerable_array_type_returns_enumerable()
        {
            Array genericArray = new Array[2];
            var intEnumerable = genericArray.ToEnumerable<int>();
            var array = genericArray.PopulateFromEnumerable(intEnumerable);
            Assert.That(array, Is.InstanceOf<Array>());
        }

        [Test]
        public void PopulateFromEnumerable_multi_array_type_returns_enumerable()
        {
            var multiArray = new[] { new Array[1], new Array[1] };
            var intEnumerable = multiArray.ToEnumerable<int>();
            var array = multiArray.PopulateFromEnumerable(intEnumerable);
            Assert.That(array, Is.InstanceOf<Array>());
        }

        [Test]
        public void PopulateFromEnumerable_array_returns_array()
        {
            _complexEnumerable = _complexArray.ToEnumerable<Complex>();
            var array = _complexArray.PopulateFromEnumerable(_complexEnumerable);
            Assert.That(array, Is.InstanceOf<Array>());
        }

        [Test]
        public void PopulateFromEnumerable_multi_2_array_returns_array()
        {
            _doubleEnumerable = _doubleArray.ToEnumerable<double>();
            var array = _doubleArray.PopulateFromEnumerable(_doubleEnumerable);
            Assert.That(array, Is.InstanceOf<Array>());
        }

        [Test]
        public void PopulateFromEnumerable_multi_3_array_returns_array()
        {
            _uShortEnumerable = _uShortArray.ToEnumerable<ushort>();
            var array = _uShortArray.PopulateFromEnumerable(_uShortEnumerable);
            Assert.That(array, Is.InstanceOf<Array>());
        }

        [Test]
        public void PopulateFromEnumerable_multi_4_array_returns_array()
        {
            _uShortEnumerable4D = _uShortArray4D.ToEnumerable<ushort>();
            var array = _uShortArray4D.PopulateFromEnumerable(_uShortEnumerable4D);
            Assert.That(array, Is.InstanceOf<Array>());
        }

        [Test]
        public void PopulateFromEnumerable_multi_5_array_returns_array()
        {
            _intEnumerable = _intArray.ToEnumerable<int>();
            var array = _intArray.PopulateFromEnumerable(_intEnumerable);
            Assert.That(array, Is.InstanceOf<Array>());
        }

        [Test]
        public void PopulateFromEnumerable_multi_6_array_returns_array()
        {
            _intEnumerable6D = _intArray6D.ToEnumerable<int>();
            var array = _intArray6D.PopulateFromEnumerable(_intEnumerable6D);
            Assert.That(array, Is.InstanceOf<Array>());
        }

        [Test]
        public void PopulateFromEnumerable_multi_7_array_returns_array()
        {
            _intEnumerable7D = _intArray7D.ToEnumerable<int>();
            var array = _intArray7D.PopulateFromEnumerable(_intEnumerable7D);
            Assert.That(array, Is.InstanceOf<Array>());
        }

        [Test]
        public void PopulateFromEnumerable2_array_type_returns_enumerable()
        {
            Array genericArray = new Array[2];
            var intEnumerable = genericArray.ToEnumerable<int>();
            var array = genericArray.PopulateFromEnumerable2(intEnumerable);
            Assert.That(array, Is.InstanceOf<Array>());
        }

        [Test]
        public void PopulateFromEnumerable2_multi_array_type_returns_enumerable()
        {
            var multiArray = new[] { new Array[1], new Array[1] };
            var intEnumerable = multiArray.ToEnumerable<int>();
            var array = multiArray.PopulateFromEnumerable2(intEnumerable);
            Assert.That(array, Is.InstanceOf<Array>());
        }

        [Test]
        public void PopulateFromEnumerable2_array_returns_array()
        {
            _complexEnumerable = _complexArray.ToEnumerable<Complex>();
            var array = _complexArray.PopulateFromEnumerable2(_complexEnumerable);
            Assert.That(array, Is.InstanceOf<Array>());
        }

        [Test]
        public void PopulateFromEnumerable2_multi_2_array_returns_array()
        {
            _doubleEnumerable = _doubleArray.ToEnumerable<double>();
            var array = _doubleArray.PopulateFromEnumerable2(_doubleEnumerable);
            Assert.That(array, Is.InstanceOf<Array>());
        }

        [Test]
        public void PopulateFromEnumerable2_multi_3_array_returns_array()
        {
            _uShortEnumerable = _uShortArray.ToEnumerable<ushort>();
            var array = _uShortArray.PopulateFromEnumerable2(_uShortEnumerable);
            Assert.That(array, Is.InstanceOf<Array>());
        }

        [Test]
        public void PopulateFromEnumerable2_multi_4_array_returns_array()
        {
            _uShortEnumerable4D = _uShortArray4D.ToEnumerable<ushort>();
            var array = _uShortArray4D.PopulateFromEnumerable2(_uShortEnumerable4D);
            Assert.That(array, Is.InstanceOf<Array>());
        }

        [Test]
        public void PopulateFromEnumerable2_multi_5_array_returns_array()
        {
            _intEnumerable = _intArray.ToEnumerable<int>();
            var array = _intArray.PopulateFromEnumerable2(_intEnumerable);
            Assert.That(array, Is.InstanceOf<Array>());
        }

        [Test]
        public void PopulateFromEnumerable2_multi_6_array_returns_array()
        {
            _intEnumerable6D = _intArray6D.ToEnumerable<int>();
            var array = _intArray6D.PopulateFromEnumerable2(_intEnumerable6D);
            Assert.That(array, Is.InstanceOf<Array>());
        }

        [Test]
        public void PopulateFromEnumerable2_multi_7_array_returns_array()
        {
            _intEnumerable7D = _intArray7D.ToEnumerable<int>();
            var array = _intArray7D.PopulateFromEnumerable2(_intEnumerable7D);
            Assert.That(array, Is.InstanceOf<Array>());
        }

        [Test]
        public void PopulateFromEnumerable2_throws_exception()
        {
            var dummyArray = new List<int>();
            Assert.Throws<ArgumentException>(() => dummyArray.PopulateFromEnumerable2(_intEnumerable7D));
        }

        [Test]
        public void PopulateWithValue_returns_array()
        {
            var complexArray = new[] { new Complex(5.0, 1.0), new Complex(2.0, 1.3) };
            var array = complexArray.PopulateWithValue(new Complex(3.0, 0.0));
            Assert.AreEqual(3.0, array[0].Real);
            Assert.AreEqual(0.0, array[0].Imaginary);
            Assert.AreEqual(3.0, array[1].Real);
            Assert.AreEqual(0.0, array[1].Imaginary);
            Assert.AreEqual(3.0, complexArray[0].Real);
            Assert.AreEqual(0.0, complexArray[0].Imaginary);
            Assert.AreEqual(3.0, complexArray[1].Real);
            Assert.AreEqual(0.0, complexArray[1].Imaginary);
        }
    }
}
