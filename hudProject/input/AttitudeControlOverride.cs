namespace hud.input
{
    internal class AttitudeControlOverride
    {
        public bool IsEnabled = false;
        public bool AnglesHaveChanged = true;

        public int HorizontalAngle = 90;
        public int VerticalAngle = 90;

        public int PreviousHorizontalAngle = 90;
        public int PreviousVerticalAngle = 90;

        public bool HasChanged()
        {
            bool horizontal = HorizontalAngle != PreviousHorizontalAngle;
            bool vertical = VerticalAngle != PreviousVerticalAngle;
            return horizontal || vertical;
        }

        public void ResetChange()
        {
            PreviousHorizontalAngle = HorizontalAngle;
            PreviousVerticalAngle = VerticalAngle;
        }
    }
}
