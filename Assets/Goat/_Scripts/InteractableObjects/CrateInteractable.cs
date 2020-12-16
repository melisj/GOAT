namespace Goat.Grid.Interactions
{
    public class CrateInteractable : StorageInteractable
    {
        public override object[] GetArgumentsForUI()
        {
            return new object[] {
            string.Format("Storage -=- {0}/{1}", GetItemCount, maxResources),
            ItemList,
            this };
        }

        protected override void Awake()
        {
            base.Awake();
        }
    }
}