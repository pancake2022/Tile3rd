using CSFramework;
using System.Collections;

public class EditorScene : BaseScene
{
    protected override IEnumerator on_init (params object[] param_list)
    {
        Framework.UIManager.OpenWindow<M3Editor>();
        yield return null;
    }

    protected override IEnumerator on_cleanup()
    {
        yield return null;
    }
}