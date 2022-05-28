namespace Skills.uncommon
{
    internal class id_1003_skillDouble_uncommon : Skill
    {
        public override void Activate()
        {
            ControllerManager.skillController.SetNextSkillDouble(true);
        }
    }
}
