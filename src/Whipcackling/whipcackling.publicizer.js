import {createPublicizer} from "publicizer";

export const publicizer = createPublicizer("Whipcackling");

publicizer.createAssembly("tModLoader").publicizeAll();
publicizer.createAssembly("CalamityMod").publicizeAll();