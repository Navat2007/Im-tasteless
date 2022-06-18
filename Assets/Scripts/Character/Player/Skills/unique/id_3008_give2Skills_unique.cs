namespace Skills.rare
{
    internal class id_3008_give2Skills_unique : Skill
    {
        public override void Activate()
        {
            var skill1 = ControllerManager.skillController.GetRandomSkill();
            ControllerManager.skillController.AddToSkillsList(skill1);
            var skill2 = ControllerManager.skillController.GetRandomSkill();
            ControllerManager.skillController.AddToSkillsList(skill2);
            
            GameUI.instance.ShowRandomSkillPanel(skill1, skill2);
        }
    }
}
