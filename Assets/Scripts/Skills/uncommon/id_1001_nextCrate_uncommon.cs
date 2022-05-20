namespace Skills.uncommon
{
    internal class id_1001_nextCrate_uncommon : Skill
    {
        public override void Activate()
        {
            ControllerManager.crateSpawner.SetPowerCrate(true, ControllerManager.skillController.IsNextDouble);
        }
    }
}
