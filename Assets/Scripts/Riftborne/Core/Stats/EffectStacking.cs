namespace Riftborne.Core.Stats
{
    public enum EffectStacking : byte
    {
        Replace = 0,   // один эффект на ключ, новый заменяет старый
        Refresh = 1,   // обновить длительность, сила не меняется
        AddStacks = 2  // накапливаем стаки (сила растёт)
    }
}