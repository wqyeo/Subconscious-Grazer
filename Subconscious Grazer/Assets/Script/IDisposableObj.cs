using System;

public interface IDisposableObj {

    event EventHandler OnObjectDisposedEvent;

    void Dispose();
}
