using System;
using System.Text.Json;

namespace Requests.Domain
{
    public interface IReconstitutable<T>
    {
       bool Reconstitute(JsonElement jsonElement);
    }
}