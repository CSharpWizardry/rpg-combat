namespace rpg_combat.Models
{
    public class AttributeModifier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Origin { get; set; }
        public bool IsPermanent { get; set; }
        //TODO: Should add these new properties?
        //Operation? Subtraction, sum, multiply, divide
        //Run out after some number of turns?
        //IsPermanent? -> different types of permanent: lost an arm? forever, got poisoned? until the end of the battle
        public bool IsPositive { get; set; } //be replaced with negative numbers???
        public bool IsUnique { get; set; }
        public int Value { get; set; }
        //Attributs and complex attributes
        public CharacterAttribute Attribute { get; set; }
        public AttributeModifier(){}
        public AttributeModifier(CharacterAttribute attribute, bool isPositive, int value)
        {
            Attribute = attribute;
            IsPositive = isPositive;
            Value = value;
        }
    }
}
