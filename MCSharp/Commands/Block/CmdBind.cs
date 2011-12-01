using System;

namespace MCSharp
{
    public class CmdBind : Command
    {
        // Constructor
        public CmdBind(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/bind <block> [type] - Replaces block with type, or unbinds a block if no type is specified.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message != "" && message.Split(' ').Length <= 2)
            {
                // Remove buildop
                if (p.BlockAction == 8) 
                { 
                    p.BlockAction = 0; 
                    p.SendMessage("Buildop Disabled, don't use /bind with it"); 
                }
 
                // Remove builddoor
                if (p.BlockAction == 9) 
                {
                    p.BlockAction = 0;
                    p.SendMessage("Builddoor Disabled, don't use /bind with it"); 
                }

                // Command Logic
                message = message.ToLower();
                int pos = message.IndexOf(' ');
                string strBlock1, strBlock2;
                byte b1, b2;

                if (pos != -1)
                {
                    strBlock1 = message.Substring(0, pos);
                    strBlock2 = message.Substring(pos + 1);
                    b1 = Block.Byte(strBlock1);

                    // Make sure the first block isn't null
                    if (b1 != 255)
                    {
                        // Make sure that the block you're trying to bind to can be placed normally
                        if (Block.Placable(b1))
                        {
                            b2 = Block.Byte(strBlock2);

                            // Make sure the second block isn't null
                            if (b2 != 255)
                            {
                                // Cannot bind air, water, and lava
                                if (b2 != 0 && b2 != 8 && b2 != 10)
                                {
                                    // Ensure it's not something they can place normally
                                    if (!Block.Placable(b2))
                                    {
                                        // Make sure the block isn't bound already
                                        if (p.bindings[b1] != b2)
                                        {
                                            // Bind the block
                                            p.bindings[b1] = b2;

                                            // Check to make the block we just bound isn't bound to a differnt block
                                            for (byte i = 0; i < 128; ++i)
                                            {
                                                byte b = i;
                                                if (p.bindings[i] == b2 && i != b1 && Block.Placable(b))
                                                {
                                                    p.SendMessage("Unbound duplicate binding: " + Block.Name(b) + ".");
                                                    p.bindings[i] = i; break;
                                                }
                                            }

                                            // Confirm to the player where they bound the block
                                            p.SendMessage(Block.Name(b1) + " bound to " + Block.Name(b2) + ".");
                                        }
                                        else
                                        {
                                            p.SendMessage(Block.Name(b1) + " is already bound to " + Block.Name(b2) + ".");
                                        }
                                    }
                                    else
                                    {
                                        p.SendMessage(Block.Name(b2) + " isn't a special block.");
                                    }
                                }
                                else
                                {
                                    p.SendMessage("You can't bind " + Block.Name(b2) + ".");
                                }
                            }
                            else
                            {
                                p.SendMessage("There is no block \"" + strBlock2 + "\".");
                            }
                        }
                        else
                        {
                            p.SendMessage("You can't bind " + Block.Name(b1) + ".");
                        }
                    }
                    else
                    {
                        p.SendMessage("There is no block \"" + strBlock1 + "\".");
                    }
                }
                else // Unbind the block
                {
                    strBlock1 = message;
                    b1 = Block.Byte(strBlock1);

                    // Make sure the block isn't null
                    if (b1 != 255)
                    {
                        // Make sure the block we're unbinding from is placeable
                        if (Block.Placable(b1))
                        {
                            // Make sure the block isn't already unbound
                            if (p.bindings[b1] != b1)
                            {
                                // Unbind and notify the player
                                p.bindings[b1] = b1;
                                p.SendMessage("Unbound " + Block.Name(b1) + ".");
                            }
                            else
                            {
                                p.SendMessage(Block.Name(b1) + " isn't bound.");
                            }
                        }
                        else
                        {
                            p.SendMessage("You can't place " + Block.Name(b1) + ".");
                        }
                    }
                    else
                    {
                        p.SendMessage("There is no block \"" + strBlock1 + "\".");
                    }
                }
            }
            else
            {
                Help(p);
            }
        }
    }
}