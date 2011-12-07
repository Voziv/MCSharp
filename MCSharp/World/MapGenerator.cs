// Copyright 2009, 2010 Matvei Stefarov <me@matvei.org>
using System;


namespace MCSharp
{

    public class MapGenerator
    {
        static bool Inuse = false;
        static float[] terrain;
        static float[] overlay;
        static float[] overlay2;
        static float divide;

        public MapGenerator()
        {

        }

        public bool GenerateMap(Level Lvl, string type)
        {
            Logger.Log("Attemping to generate a map. Type: " + type);
            if (Inuse) 
            { 
                Logger.Log("Generator in use", LogType.Warning);
                return false;
            }
            
            Random rand = new System.Random();
            try
            {
                Inuse = true;
                terrain = new float[Lvl.width * Lvl.height];
                overlay = new float[Lvl.width * Lvl.height];

                if (!type.Equals("ocean"))
                { overlay2 = new float[Lvl.width * Lvl.height]; }

                //float dispAux, pd;
                ushort WaterLevel = (ushort)(Lvl.depth / 2 + 2);

                if (type.Equals("ocean"))
                {
                    WaterLevel = (ushort)(Lvl.depth * 0.85f);
                }

                //Generate the level
                GenerateFault(terrain, Lvl, type, rand);

                //APPLY FILTER to terrain
                FilterAverage(Lvl);

                //CREATE OVERLAY
                //GenerateFault(overlay, Lvl, "overlay", rand);
                Logger.Log("Creating overlay", LogType.Debug);
                GeneratePerlinNoise(overlay, Lvl, "", rand);

                if (!type.Equals("ocean"))
                {
                    Logger.Log("Planning trees", LogType.Debug);
                    GeneratePerlinNoise(overlay2, Lvl, "", rand);
                }

                Logger.Log("Converting height map", LogType.Debug);
                Logger.Log("And applying overlays", LogType.Debug);
                float RangeLow = 0.2f;
                float RangeHigh = 0.8f;
                float TreeDens = 0.35f;
                short TreeDist = 3;
                //changes the terrain range based on type, also tree threshold
                switch(type)
                {
                    case "island":
                        RangeLow = 0.4f;
                        RangeHigh = 0.75f;
                        break;
                    case "forest":
                        RangeLow = 0.45f;
                        RangeHigh = 0.8f;
                        TreeDens = 0.7f;
                        TreeDist = 2;
                        break;
                    case "mountains":
                        RangeLow = 0.3f;
                        RangeHigh = 0.9f;
                        TreeDist = 4;
                        break;
                    case "ocean":
                        RangeLow = 0.1f;
                        RangeHigh = 0.6f;
                        break;
                    default:
                        break;
                }

                //loops though evey X/Z coordinate
                for ( int bb = 0; bb < terrain.Length; bb++)
                {
                    ushort x = (ushort)(bb % Lvl.width);
                    ushort y = (ushort)(bb / Lvl.width);
                    ushort z;
                    if (type.Equals("island"))  //apply the edge dip
                    {
                        z = Evaluate(Lvl, Range(terrain[bb], RangeLow - NegateEdge(x, y, Lvl), RangeHigh - NegateEdge(x, y, Lvl)));
                    }
                    else
                    {
                        z = Evaluate(Lvl, Range(terrain[bb], RangeLow, RangeHigh));
                    }
                    if (z > WaterLevel)
                    {
                        for (ushort zz = 0; z - zz >= 0; zz++)
                        {
                            if (overlay[bb] < 0.72f)    //If not zoned for rocks or gravel
                            {
                                if (type.Equals("island"))      //increase sand height for island
                                {
                                    if (z > WaterLevel + 2)
                                    {
                                        if (zz == 0) { Lvl.Blockchange(x, (ushort)(z - zz), y, Block.grass); }      //top layer
                                        else if (zz < 3) { Lvl.Blockchange(x, (ushort)(z - zz), y, Block.dirt); }   //next few
                                        else { Lvl.Blockchange(x, (ushort)(z - zz), y, Block.rock); }               //ten rock it
                                    }
                                    else
                                    {
                                        Lvl.Blockchange(x, (ushort)(z - zz), y, Block.sand);                        //SAAAND extra for islands
                                    }
                                }
                                else
                                {
                                    if (zz == 0) { Lvl.Blockchange(x, (ushort)(z - zz), y, Block.grass); }
                                    else if (zz < 3) { Lvl.Blockchange(x, (ushort)(z - zz), y, Block.dirt); }
                                    else { Lvl.Blockchange(x, (ushort)(z - zz), y, Block.rock); }
                                }
                            }
                            else
                            {
                                Lvl.Blockchange(x, (ushort)(z - zz), y, Block.rock);    //zoned for above sea level rock floor
                            }
                        }

                        if (overlay[bb] < 0.25f)    //Zoned for flowers
                        {
                            int temprand = rand.Next(12);

                            switch (temprand)
                            {
                                case 10:
                                    Lvl.Blockchange(x, (ushort)(z + 1), y, Block.redflower);
                                    break;
                                case 11:
                                    Lvl.Blockchange(x, (ushort)(z + 1), y, Block.yellowflower);
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (!type.Equals("ocean"))  //Oceans dont have trees
                        {
                            if (overlay[bb] < 0.65f && overlay2[bb] < TreeDens) //If not a rock zoned area and is zoned for possible trees
                            {
                                if (Lvl.GetTile(x, (ushort)(z + 1), y) == Block.air)    //No flowers here right?
                                {
                                    if (Lvl.GetTile(x, z, y) == Block.grass)    //Im not on sand am I either?
                                    {
                                        if (rand.Next(13) == 0)
                                        {
                                            if (!TreeCheck(Lvl, x, z, y, TreeDist))       //Am I too close to other trees?
                                            { AddTree(Lvl, x, (ushort)(z + 1), y, rand); }
                                        }
                                    }
                                }
                            }
                        }

                    }
                    else    //Must be on/under the water line then
                    {
                        for (ushort zz = 0; WaterLevel - zz >= 0; zz++)
                        {
                            if (WaterLevel - zz > z)
                            {Lvl.Blockchange(x, (ushort)(WaterLevel - zz), y, Block.water);}    //better fill the water aboce me
                            else if (WaterLevel - zz > z-3)
                            {
                                if (overlay[bb] < 0.75f)
                                {
                                    Lvl.Blockchange(x, (ushort)(WaterLevel - zz), y, Block.sand);   //sand top
                                }
                                else
                                {
                                    Lvl.Blockchange(x, (ushort)(WaterLevel - zz), y, Block.gravel);  //zoned for gravel
                                }
                            }
                            else
                            {Lvl.Blockchange(x, (ushort)(WaterLevel - zz), y, Block.rock);}
                        }
                    }
                }

            }
            catch
            {
                Logger.Log("Map Generator Failed", LogType.Error);
                Inuse = false;
                return false;
            }

            terrain = new float[0]; //Derp
            overlay = new float[0]; //Derp
            overlay2 = new float[0]; //Derp

            Inuse = false;

            return true;
        }

        //condensed fault generator
        #region ==FaultGen==
        void GenerateFault(float[] array,Level Lvl, string type, Random rand)
        {
            float startheight = 0.5f;
            float dispAux;
            ushort i, j, k, halfX, halfZ;
            float a, b, c, w, d;

            float DispMax, DispMin, DispChange;
            DispMax = 0.01f;
            DispChange = -0.0025f;
            if (type.Equals("mountains"))
            {
                DispMax = 0.02f;
                startheight = 0.6f;
            }
            else if (type.Equals("overlay"))
            {
                DispMax = 0.02f;
                DispChange = -0.01f;
            }

            for (int x = 0; x < array.Length; x++)
            {
                array[x] = startheight;
                //overlay[x] = 0.5f;
            }
            DispMin = -DispMax;
            float disp = DispMax;
            //if (terrainHeights == NULL)
            //    return (TERRAIN_ERROR_NOT_INITIALISED);


            halfX = (ushort)(Lvl.width / 2);
            halfZ = (ushort)(Lvl.height / 2);
            int numIterations = (int)((Lvl.width + Lvl.height));
            Logger.Log("Iterations = " + numIterations.ToString(), LogType.Debug);
            for (k = 0; k < numIterations; k++)
            {
                //s.Log("itteration " + k.ToString());
                d = (float)Math.Sqrt(halfX * halfX + halfZ * halfZ);
                w = (float)(rand.NextDouble() * 360);
                //w = (float)(rand.NextDouble()*90);
                a = (float)Math.Cos(w);
                b = (float)Math.Sin(w);

                c = ((float)rand.NextDouble()) * 2 * d - d;
                //c = ((float)rand.NextDouble() / 1) * 2 * d - d;
                //float disp = (float)(rand.NextDouble()* 0.02f - 0.01f);
                //iterationsDone++;
                //if (iterationsDone < itMinDisp)
                //    disp = maxDisp + (iterationsDone / (itMinDisp + 0.0)) * (minDisp - maxDisp);
                //else
                //    disp = minDisp;
                for (i = 0; i < Lvl.height; i++)
                {
                    for (j = 0; j < Lvl.width; j++)
                    {
                        //switch (terrainFunction)
                        //{
                        //case STEP:
                        if ((i - halfZ) * a + (j - halfX) * b + c > 0)
                            dispAux = disp;
                        else
                            dispAux = -disp;
                        //    break;
                        /*case SIN:
                            pd = ((i - halfZ) * a + (j - halfX) * b + c) / terrainWaveSize;
                            if (pd > 1.57) pd = 1.57;
                            else if (pd < 0) pd = 0;
                            dispAux = -disp / 2 + sin(pd) * disp;
                            break;
                        case COS:
                            pd = ((i - halfZ) * a + (j - halfX) * b + c) / terrainWaveSize;
                            if (pd > 3.14) pd = 3.14;
                            else if (pd < -3.14) pd = -3.14;
                            dispAux = disp - (terrainWaveSize / (terrainGridWidth + 0.0)) + cos(pd) * disp;
                            break;
                    }*/
                        //s.Log("adding " + dispAux.ToString());
                        AddTerrainHeight(array, j, i, Lvl.width, dispAux);
                        //terrainHeights[i * terrainGridWidth + j] += dispAux;
                    }
                }

                disp += DispChange;
                if (disp < DispMin) { disp = DispMax; }
            }
        }
        #endregion

        //hur hur, more copy pasted code :/
        #region ==PerlinGen==
        void GeneratePerlinNoise(float[] array, Level Lvl, string type, Random rand)
        {
            GenerateNormalized(array, 0.7f, 8, Lvl.width, Lvl.height, rand.Next(), 64);
        }

        void GenerateNormalized(float[] array, float persistence, int octaves, int width, int height, int seed, float zoom)
        {	
	        float min = 0;
	        float max = 0;
	        //float * pDataFloat = new float[width * height];

	        //Generate raw float data
	        for (int y = 0; y < height; ++y)
	        {
		        for (int x = 0; x < width; ++x)
		        {
			        float total = 0;
			        float frequency = 1;
			        float amplitude = 1;

			        for (int i = 0; i < octaves; ++i)
			        {
				        total = total + InterpolatedNoise(x * frequency / zoom, y * frequency / zoom, seed) * amplitude;
				        frequency *= 2;
				        amplitude *= persistence;
			        }

                    array[y * width + x] = total;

			        min = total < min ? total : min;
			        max = total > max ? total : max;
		        }
	        }

	        //Normalize
	        for (int i = 0; i < width * height; ++i)
	        {
                array[i] = (array[i] - min) / (max - min);
			    //array[i] = (255 << 24) | ((unsigned char) (red * ((pDataFloat[i] - min) / (max - min)) * 255) << 16) | 
				//((unsigned char) (green * ((pDataFloat[i] - min) / (max - min)) * 255) << 8) | (unsigned char) (blue * ((pDataFloat[i] - min) / (max - min)) * 255);
	        }
        }

        float Noise(int x, int y, int seed)
        {
            int n = x + y * 57 + seed;
            n = (n << 13) ^ n;
            return (float)(1.0 - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0);
            //return value is always in range [-1.0, 1.0]
        }

        float SmoothNoise(int x, int y, int seed)
        {
            float corners = (Noise(x - 1, y - 1, seed) + Noise(x + 1, y - 1, seed) + Noise(x - 1, y + 1, seed) + Noise(x + 1, y + 1, seed)) / 16;
            float sides = (Noise(x - 1, y, seed) + Noise(x + 1, y, seed) + Noise(x, y - 1, seed) + Noise(x, y + 1, seed) / 8);
            float center = Noise(x, y, seed) / 4;
            return corners + sides + center;
        }

        float Interpolate(float a, float b, float x)
        {
	        float ft = x * 3.1415927f;
	        float f = (float)(1 - Math.Cos(ft)) * .5f;

	        return  a*(1-f) + b*f;
        }

        float InterpolatedNoise(float x, float y, int seed)
        {
            int wholePartX = (int)x;
            float fractionPartX = x - wholePartX;

            int wholePartY = (int)y;
            float fractionPartY = y - wholePartY;

            float v1 = SmoothNoise(wholePartX, wholePartY, seed);
            float v2 = SmoothNoise(wholePartX + 1, wholePartY, seed);
            float v3 = SmoothNoise(wholePartX, wholePartY + 1, seed);
            float v4 = SmoothNoise(wholePartX + 1, wholePartY + 1, seed);

            float i1 = Interpolate(v1, v2, fractionPartX);
            float i2 = Interpolate(v3, v4, fractionPartX);

            return Interpolate(i1, i2, fractionPartY);
        }

        #endregion

        //
        void AddTree(Level Lvl, ushort x, ushort z, ushort y, Random Rand)
        {
            byte height = (byte)Rand.Next(4, 7);
            for (ushort zz = 0; zz < height; zz++)
            {
                if (Lvl.GetTile(x, (ushort)(z + zz), y) == Block.air)   //Not likly to trigger anyway
                {
                    Lvl.Blockchange(x, (ushort)(z + zz), y, Block.trunk);
                }
                else
                {
                    height = (byte)zz;
                }
            }

            short top = (short)(height - 3);

            for (short xx = (short)-top; xx <= top; ++xx)
            {
                for (short yy = (short)-top; yy <= top; ++yy)
                {
                    for (short zz = (short)-top; zz <= top; ++zz)
                    {
                        if (Lvl.GetTile((ushort)(x + xx), (ushort)(z + zz + height), (ushort)(y + yy)) == Block.air)   //Not likly to trigger anyway
                        {
                            //short Dist = (short)(Math.Abs(xx) + Math.Abs(yy) + Math.Abs(zz));
                            short Dist = (short)(Math.Sqrt(xx * xx + yy * yy + zz * zz));
                            if (Dist < top + 1)
                            {
                                if (Rand.Next((int)(Dist)) < 2)
                                {
                                    Lvl.Blockchange((ushort)(x + xx), (ushort)(z + zz + height), (ushort)(y + yy), Block.leaf);
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool TreeCheck(Level Lvl, ushort x, ushort z, ushort y, short dist)         //return true if tree is near
        {
            for (short xx = (short)-dist; xx <= +dist; ++xx)
            {
                for (short yy = (short)-dist; yy <= +dist; ++yy)
                {
                    for (short zz = (short)-dist; zz <= +dist; ++zz)
                    {
                        if (Lvl.GetTile((ushort)(x + xx), (ushort)(z + zz ), (ushort)(y + yy)) == Block.trunk)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        void AddTerrainHeight(float[] array, ushort x, ushort y, ushort width, float height)
        {
            int temp = x + y * width;
            if (temp < 0) return;
            if (temp > array.Length) return;

            array[temp] += height;

            if (array[temp] > 1.0f) array[temp] = 1.0f;
            if (array[temp] < 0.0f) array[temp] = 0.0f;
        }

        //converts the float into a ushort for map height
        ushort Evaluate(Level lvl, float height)
        {
            ushort temp = (ushort)(height * lvl.depth);
            if (temp < 0) return 0;
            if (temp > lvl.depth - 1) return (ushort)(lvl.depth - 1);
            return temp;
        }

        //applys the average filter
        void FilterAverage(Level Lvl)
        {
            Logger.Log("Applying average filtering", LogType.Debug);

            float[] filtered = new float[terrain.Length];

            for (int bb = 0; bb < terrain.Length; bb++)
            {
                ushort x = (ushort)(bb % Lvl.width);
                ushort y = (ushort)(bb / Lvl.width);
                filtered[bb] = GetAverage9(x, y, Lvl);
            }

            for (int bb = 0; bb < terrain.Length; bb++)
            {
                terrain[bb] = filtered[bb];
            }
        }

        //Averages over 5 points
        float GetAverage5(ushort x, ushort y, Level Lvl)
        {
            divide = 0.0f;
            float temp = GetPixel(x, y, Lvl);
            temp += GetPixel((ushort)(x + 1), y, Lvl);
            temp += GetPixel((ushort)(x - 1), y, Lvl);
            temp += GetPixel(x, (ushort)(y + 1), Lvl);
            temp += GetPixel(x, (ushort)(y - 1), Lvl);

            return temp / divide;
        }
        //Averages over 9 points
        float GetAverage9(ushort x, ushort y, Level Lvl)
        {
            divide = 0.0f;
            float temp = GetPixel(x, y, Lvl);
            temp += GetPixel((ushort)(x + 1), y, Lvl);
            temp += GetPixel((ushort)(x - 1), y, Lvl);
            temp += GetPixel(x, (ushort)(y + 1), Lvl);
            temp += GetPixel(x, (ushort)(y - 1), Lvl);

            temp += GetPixel((ushort)(x + 1), (ushort)(y + 1), Lvl);
            temp += GetPixel((ushort)(x - 1), (ushort)(y + 1), Lvl);
            temp += GetPixel((ushort)(x + 1), (ushort)(y - 1), Lvl);
            temp += GetPixel((ushort)(x - 1), (ushort)(y - 1), Lvl);

            return temp / divide;
        }

        //returns the valve of a x,y terrain coordinate
        float GetPixel(ushort x, ushort y, Level Lvl)
        {
            if (x < 0) { return 0.0f; }
            if (x >= Lvl.width) { return 0.0f; }
            if (y < 0) { return 0.0f; }
            if (y >= Lvl.height) { return 0.0f;}
            divide += 1.0f;
            return terrain[x + y * Lvl.width];
        }

        //converts the height into a range
        float Range(float input, float low, float high)
        {
            if (high <= low) { return low; }
            return low + (input * (high - low));
        }

        //Forces the edge of a map to slope lower for island map types
        float NegateEdge(ushort x, ushort y, Level Lvl)
        {
            float tempx = 0.0f, tempy = 0.0f;
            float temp;
            if (x != 0) { tempx = ((float)x / (float)Lvl.width) * 0.5f; }
            if (y != 0) { tempy = ((float)y / (float)Lvl.height) * 0.5f; }
            tempx = Math.Abs(tempx - 0.25f);
            tempy = Math.Abs(tempy - 0.25f);
            if (tempx > tempy)
            {
                temp = tempx - 0.15f;
            }
            else
            {
                temp = tempy - 0.15f;
            }

            //s.Log("temp = " + temp.ToString());
            if (temp > 0.0f) { return temp; }
            return 0.0f;
        }
    }
}