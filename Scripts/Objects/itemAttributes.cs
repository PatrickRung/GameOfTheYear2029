using System;

//all item data will be stores in objects labled itemAttributes
public class itemAttributes {
    public String name;
    public Boolean isWeapon, isConsumable;
    public int damageValue, healValue;
    public itemAttributes(String name, Boolean isWeapon, Boolean isConsumable) {
        this.name = name;
        this.isWeapon = isWeapon;
        this.isConsumable = isConsumable;
    }
}