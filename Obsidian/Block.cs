﻿using Obsidian.API;
using Obsidian.Utilities.Registry;
using Obsidian.WorldData;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Obsidian
{
    [DebuggerDisplay("{Name,nq}:{Id}")]
    public readonly struct Block : IEquatable<Block>
    {
        public static Block Air => new Block(0, 0);

        private static short[] interactables;
        private static bool initialized = false;

        public string UnlocalizedName => Registry.BlockNames[Id];
        public string Name => Material.ToString();
        public Material Material => (Material)Registry.StateToMatch[baseId].numeric;
        public bool IsInteractable => (baseId >= 9276 && baseId <= 9372) || Array.BinarySearch(interactables, baseId) > -1;
        public bool IsAir => baseId == 0 || baseId == 9670 || baseId == 9669;
        public bool IsFluid => StateId > 33 && StateId < 66;
        public int Id => Registry.StateToMatch[baseId].numeric;
        public short StateId => (short)(baseId + state);
        public int State => state;
        public short BaseId => baseId;

        private readonly short baseId;
        private readonly short state;

        internal static readonly List<Material> Replaceable = new()
        {
            Material.Air,
            Material.CaveAir,
            Material.Cobweb,
            Material.SugarCane,
            Material.DeadBush,
            Material.Grass,
            Material.TallGrass,
            Material.AcaciaSapling,
            Material.BambooSapling,
            Material.BirchSapling,
            Material.DarkOakSapling,
            Material.JungleSapling,
            Material.OakSapling,
            Material.SpruceSapling,
            Material.Dandelion,
            Material.Poppy,
            Material.BlueOrchid,
            Material.Allium,
            Material.AzureBluet,
            Material.OrangeTulip,
            Material.PinkTulip,
            Material.RedTulip,
            Material.WhiteTulip,
            Material.OxeyeDaisy,
            Material.Cornflower,
            Material.LilyOfTheValley,
            Material.WitherRose,
            Material.Sunflower,
            Material.Lilac,
            Material.Sunflower,
            Material.Peony,
            Material.Wheat,
            Material.PumpkinStem,
            Material.Carrots,
            Material.Potatoes,
            Material.Beetroots,
            Material.SweetBerryBush
        };

        internal static readonly List<Material> GravityAffected = new()
        {
            Material.Anvil,
            Material.ChippedAnvil,
            Material.DamagedAnvil,
            Material.WhiteConcretePowder,
            Material.OrangeConcretePowder,
            Material.MagentaConcretePowder,
            Material.LightBlueConcretePowder,
            Material.YellowConcretePowder,
            Material.LimeConcretePowder,
            Material.PinkConcretePowder,
            Material.GrayConcretePowder,
            Material.LightGrayConcretePowder,
            Material.CyanConcretePowder,
            Material.PurpleConcretePowder,
            Material.BlueConcretePowder,
            Material.BrownConcretePowder,
            Material.GreenConcretePowder,
            Material.RedConcretePowder,
            Material.BlackConcretePowder,
            Material.DragonEgg,
            Material.RedSand,
            Material.Sand,
            Material.Scaffolding
        };

        public Block(int stateId) : this((short)stateId)
        {
        }

        public Block(short stateId)
        {
            baseId = Registry.StateToMatch[stateId].@base;
            state = (short)(stateId - baseId);
        }

        public Block(int baseId, int state) : this((short)baseId, (short)state)
        {
        }

        public Block(short baseId, short state)
        {
            this.baseId = baseId;
            this.state = state;
        }

        public Block(Material material, short state = 0)
        {
            baseId = Registry.NumericToBase[(int)material];
            this.state = state;
        }

        public override string ToString()
        {
            return UnlocalizedName;
        }

        public override int GetHashCode()
        {
            return StateId;
        }

        public bool Is(Material material)
        {
            return Registry.StateToMatch[baseId].numeric == (int)material;
        }

        public override bool Equals(object obj)
        {
            return (obj is Block block) && block.StateId == StateId;
        }

        public bool Equals(Block other)
        {
            return other.StateId == StateId;
        }

        public static bool operator ==(Block a, Block b)
        {
            return a.StateId == b.StateId;
        }

        public static bool operator !=(Block a, Block b)
        {
            return a.StateId != b.StateId;
        }

        internal static void Initialize()
        {
            if (initialized)
                return;
            initialized = true;

            interactables = new[]
            {
                Registry.NumericToBase[(int)Material.Chest],
                Registry.NumericToBase[(int)Material.CraftingTable],
                Registry.NumericToBase[(int)Material.Furnace],
                Registry.NumericToBase[(int)Material.BrewingStand],
                Registry.NumericToBase[(int)Material.EnderChest],
                Registry.NumericToBase[(int)Material.Anvil],
                Registry.NumericToBase[(int)Material.ChippedAnvil],
                Registry.NumericToBase[(int)Material.DamagedAnvil],
                Registry.NumericToBase[(int)Material.TrappedChest],
                Registry.NumericToBase[(int)Material.Hopper],
                Registry.NumericToBase[(int)Material.Barrel],
                Registry.NumericToBase[(int)Material.Smoker],
                Registry.NumericToBase[(int)Material.BlastFurnace],
                Registry.NumericToBase[(int)Material.Grindstone],
            };
        }
    }

    public struct BlockUpdate
    {
        internal readonly World world;
        internal Vector position;
        internal int delay { get; private set; }
        internal int delayCounter;
        private Block? _block;
        internal Block? block
        {
            get => _block;
            set 
            {
                _block = value;
                if (value is Block b)
                {
                    if (Block.GravityAffected.Contains(b.Material))
                    {
                        delay = 1;
                    }
                    else if (b.Material == Material.Lava)
                    {
                        delay = 40;
                    }
                    else if (b.Material == Material.Water)
                    {
                        delay = 5;
                    }
                }
                delayCounter = delay;
            }
        }

        public BlockUpdate(World w, Vector pos, Block? blk = null)
        {
            world = w;
            position = pos;
            delay = 0;
            delayCounter = delay;
            _block = null;
            block = blk;
        }
    }
}
