namespace Skills.rare
{
    internal class id_3010_powerBusters_unique : Skill
    {
        public override void Activate()
        {
            ControllerManager.busterController.SetDoubleBuster(true);
        }
    }
}
