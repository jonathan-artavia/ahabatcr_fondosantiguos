using Fondos_Antiguos.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;

namespace Fondos_Antiguos.Base
{
    public class BaseDataService : IDisposable
    {
        private static readonly Lazy<MemoryCache> lazy =
            new Lazy<MemoryCache>
                (() => new MemoryCache(new MemoryCacheOptions() { CompactionPercentage = 0.5, ExpirationScanFrequency = TimeSpan.FromHours(1) }));
        private bool disposedValue;

        public static MemoryCache DataCache { get { return lazy.Value; } }

        public BaseDataService()
        {
            
        }

        protected virtual object GetOrCreateKey(ApplicationUser user)
        {
            object key = (user?.UserName ?? "anon");
            //DataCache.GetOrCreate<Dictionary<string, object>>(key, (x) => new Dictionary<string, object>() { { "OtherKeys", keys }, { "Value", null } });
            return key;
        }

        protected virtual object GetOrCreateKey(HttpContextBase context)
        {
            object key = (context.User?.Identity?.GetUserId() ?? "anon");
            //DataCache.GetOrCreate<Dictionary<string, object>>(key, (x) => new Dictionary<string, object>() { { "OtherKeys", keys }, { "Value", null } });
            return key;
        }

        protected virtual T GetOrCreateValue<T>(object key, Func<T> value, [CallerMemberName] string method = null, params object[] otherKeys)
        {
            return this.CreateOrUpdateEntry<T>(key, method, value, otherKeys);
        }

        protected virtual T GetOrCreateValue<T>(object key, string method = null, params object[] otherKeys)
        {
            Dictionary<string, object> reg = DataCache.Get<Dictionary<string, object>>(key);

            T GetValueFromInnerReg()
            {
                if (reg.Where(x => x.Key.EndsWith("_Keys"))?.FirstOrDefault(y => this.AreOtherKeysEqual((object[])y.Value, otherKeys)) == null)
                    return default(T);
                if (!reg.ContainsKey($"Method_{method}_Last"))
                    return default(T);
                int last = (int)reg[$"Method_{method}_Last"];
                for (int i = 0; i <= last; i++)
                {
                    if (otherKeys != null && this.AreOtherKeysEqual((object[])reg[$"Method_{method}{i}_Keys"], otherKeys))
                    {
                        return (T)reg[$"Method_{method}{i}_Values"];
                    }
                }
                return default(T);
            }

            T GetValueFromInnerRegIndex(int index)
            {
                if (otherKeys != null && this.AreOtherKeysEqual((object[])reg[$"Method_{method}{index}_Keys"], otherKeys))
                {
                    return (T)reg[$"Method_{method}{index}_Values"];
                }
                return default(T);
            }

            if (reg == null)
            {
                return default(T);
            }
            else
            {
                T probe = GetValueFromInnerReg();
                T defaultVal = default(T);
                if (probe == null || probe.Equals(defaultVal))
                {
                    return default(T);
                }
                return probe;
            }
            //return GetValueFromInnerRegIndex(lookupRegisterIndex);
        }

        protected virtual void RemoveValueIfExists(object key, string method, params object[] otherKeys)
        {
            Dictionary<string, object> reg = DataCache.Get<Dictionary<string, object>>(key);

            if (reg == null || (!string.IsNullOrEmpty(method) && !reg.ContainsKey($"Method_{method}_Last")))
                return;

            if(string.IsNullOrEmpty(method))
            {
                DataCache.Remove(key);
                return;
            }

            int last = (int)reg[$"Method_{method}_Last"];
            if(otherKeys == null || otherKeys.Length == 0)
            {
                for (int i = 0; i <= last; i++)
                {
                    reg.Remove($"Method_{method}{i}_Keys");
                    reg.Remove($"Method_{method}{i}_Values");
                }
                reg.Remove($"Method_{method}_Last");
                DataCache.Set(key, reg); //udpates the cache registry
                return;
            }

            for (int i = 0; i <= last; i++)
            {
                if (this.AreOtherKeysEqual((object[])reg[$"Method_{method}{i}_Keys"], otherKeys))
                {
                    reg.Remove($"Method_{method}{i}_Keys");
                    reg.Remove($"Method_{method}{i}_Values");
                    if (i == last)
                        last--;
                }
            }
            reg[$"Method_{method}_Last"] = last;
            DataCache.Set(key, reg); //udpates the cache registry
        }

        private T CreateOrUpdateEntry<T>(object key, string method, Func<T> value, params object[] otherkeys)
        {
            Dictionary<string, object> reg = DataCache.Get< Dictionary<string, object>>(key);
            
            int AddToEntryReg()
            {
                int last = 0;
                if (reg.ContainsKey($"Method_{method}_Last"))
                {
                    last = (int)reg[$"Method_{method}_Last"];
                    last++;
                    if (otherkeys != null)
                    {
                        if (reg.ContainsKey($"Method_{method}{last}_Keys"))
                            reg[$"Method_{method}{last}_Keys"] = otherkeys;
                        else
                            reg.Add($"Method_{method}{last}_Keys", otherkeys);
                    }
                    reg.Add($"Method_{method}{last}_Values", value());
                    reg[$"Method_{method}_Last"] = last;
                    return last;
                }
                else
                {
                    if (otherkeys != null)
                    {
                        if (!reg.ContainsKey($"Method_{method}0_Keys"))
                            reg.Add($"Method_{method}0_Keys", otherkeys);
                        else
                            reg[$"Method_{method}0_Keys"] = otherkeys;
                    }
                    else
                    {
                        if (reg.ContainsKey($"Method_{method}0_Keys"))
                            reg.Remove($"Method_{method}0_Keys");
                    }
                    if (!reg.ContainsKey($"Method_{method}0_Values"))
                        reg.Add($"Method_{method}0_Values", value());
                    else
                        reg[$"Method_{method}0_Values"] = value();
                    if (reg.ContainsKey($"Method_{method}_Last"))
                        reg.Add($"Method_{method}_Last", 0);
                    else
                        reg[$"Method_{method}_Last"] = 0;
                    return 0;
                }
            }

            T GetValueFromInnerReg()
            {
                if (reg.Where(x => x.Key.EndsWith("_Keys"))?.FirstOrDefault(y => this.AreOtherKeysEqual((object[])y.Value, otherkeys)) == null)
                    return default(T);
                if (!reg.ContainsKey($"Method_{method}_Last"))
                    return default(T);
                int last = (int)reg[$"Method_{method}_Last"];
                for(int i=0; i <= last; i++)
                {
                    if(otherkeys != null && this.AreOtherKeysEqual((object[])reg[$"Method_{method}{i}_Keys"], otherkeys))
                    {
                        return (T)reg[$"Method_{method}{i}_Values"];
                    }
                }
                return default(T);
            }

            T GetValueFromInnerRegIndex(int index)
            {
                if(otherkeys != null && this.AreOtherKeysEqual((object[])reg[$"Method_{method}{index}_Keys"],otherkeys))
                {
                    return (T)reg[$"Method_{method}{index}_Values"];
                }
                return default(T);
            }

            int lookupRegisterIndex = 0;


            if (reg == null)
            {
                reg = this.CreateEntry();
                lookupRegisterIndex = AddToEntryReg();
                DataCache.GetOrCreate<Dictionary<string, object>>(key, x =>
                {
                    x.SetAbsoluteExpiration(new TimeSpan(1, 0, 0, 0));
                    return reg;
                });
            }
            else
            {
                T probe = GetValueFromInnerReg();
                T defaultVal = default(T);
                if (probe == null || probe.Equals(defaultVal))
                {
                    lookupRegisterIndex = AddToEntryReg(); //adds the missing entry in reg, for the method, with those otherKeys
                    DataCache.Set(key, reg, DateTime.Now.AddDays(1)); //udpates the cache registry
                    probe = GetValueFromInnerRegIndex(lookupRegisterIndex);
                }
                return probe;
            }
            return GetValueFromInnerRegIndex(lookupRegisterIndex);
        }

        private Dictionary<string, object> CreateEntry()
        {
            return new Dictionary<string, object>();
        }

        private bool AreOtherKeysEqual(object[] first, object[] second)
        {
            if (first.Length != second.Length)
                return false;

            for(int i=0; i < first.Length; i++)
            {
                if (!Object.Equals(first[i], second[i]) && (!(first[i] is System.Collections.IDictionary) || !(second[i] is System.Collections.IDictionary) || !((System.Collections.IDictionary)first[i]).Cast<object>().SequenceEqual(((System.Collections.IDictionary)second[i]).Keys.Cast<object>())))
                    return false;
            }
            return true;
        }

        #region Dispose Pattern
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~BaseDataService()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        } 
        #endregion
    }
}
