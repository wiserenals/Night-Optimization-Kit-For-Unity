using UnityEngine;

public static class TerrainDataCloner
{
    		/// <summary>
		/// Creates a real deep-copy of a TerrainData
		/// </summary>
		/// <param name="original">TerrainData to duplicate</param>
		/// <returns>New terrain data instance</returns>
		public static TerrainData Clone(TerrainData source)
		{
			
			TerrainData clonedData = new TerrainData
			{
				heightmapResolution = source.heightmapResolution,
				alphamapResolution = source.alphamapResolution,
				baseMapResolution = source.baseMapResolution,
				detailPrototypes = CloneDetailPrototypes(source.detailPrototypes),
				wavingGrassAmount = source.wavingGrassAmount,
				wavingGrassSpeed = source.wavingGrassSpeed,
				wavingGrassStrength = source.wavingGrassStrength,
				wavingGrassTint = source.wavingGrassTint,
				treePrototypes = CloneTreePrototypes(source.treePrototypes),
				treeInstances = CloneTreeInstances(source.treeInstances),
				terrainLayers = source.terrainLayers,
				size = source.size
			};

			clonedData.SetDetailResolution(source.detailResolution, 16);
			// Clone heightmap
			clonedData.SetHeights(0, 0, source.GetHeights(0, 0, source.heightmapResolution, source.heightmapResolution));
			// Debugging for alphamap layers
			
			Debug.Log($"Source Alphamap Layers: {source.alphamapLayers}, Cloned Alphamap Layers: {clonedData.alphamapLayers}");

			// Clone alphamap data
			float[,,] alphaMaps = source.GetAlphamaps(0, 0, source.alphamapResolution, source.alphamapResolution);
			if (clonedData.alphamapLayers == source.alphamapLayers)
			{
				clonedData.SetAlphamaps(0, 0, alphaMaps);
			}
			else
			{
				Debug.LogError("Alphamap layer mismatch. Adjust the clonedData layers.");
			}

			// Clone detail layers
			for (int i = 0; i < source.detailPrototypes.Length; i++)
			{
				int[,] detailLayer = source.GetDetailLayer(0, 0, source.detailWidth, source.detailHeight, i);
				clonedData.SetDetailLayer(0, 0, i, detailLayer);
			}

			return clonedData;
		}

		/// <summary>
		/// Deep-copies an array of detail prototype instances
		/// </summary>
		/// <param name="original">Prototypes to clone</param>
		/// <returns>Cloned array</returns>
            private static DetailPrototype[] CloneDetailPrototypes(DetailPrototype[] original)
		{
			DetailPrototype[] protoDuplicate = new DetailPrototype[original.Length];

			for (int n = 0; n < original.Length; n++)
			{
				protoDuplicate[n] = new DetailPrototype
				{
					dryColor = original[n].dryColor,
					healthyColor = original[n].healthyColor,
					maxHeight = original[n].maxHeight,
					maxWidth = original[n].maxWidth,
					minHeight = original[n].minHeight,
					minWidth = original[n].minWidth,
					noiseSpread = original[n].noiseSpread,
					prototype = original[n].prototype,
					prototypeTexture = original[n].prototypeTexture,
					renderMode = original[n].renderMode,
					usePrototypeMesh = original[n].usePrototypeMesh,
					useInstancing = original[n].useInstancing,
				};
			}

			return protoDuplicate;
		}

		/// <summary>
		/// Deep-copies an array of tree prototype instances
		/// </summary>
		/// <param name="original">Prototypes to clone</param>
		/// <returns>Cloned array</returns>
		private static TreePrototype[] CloneTreePrototypes(TreePrototype[] original)
		{
			TreePrototype[] protoDuplicate = new TreePrototype[original.Length];

			for (int n = 0; n < original.Length; n++)
			{
				protoDuplicate[n] = new TreePrototype
				{
					bendFactor = original[n].bendFactor,
					prefab = original[n].prefab,
				};
			}

			return protoDuplicate;
		}

		/// <summary>
		/// Deep-copies an array of tree instances
		/// </summary>
		/// <param name="original">Trees to clone</param>
		/// <returns>Cloned array</returns>
		private static TreeInstance[] CloneTreeInstances(TreeInstance[] original)
		{
			TreeInstance[] treeInst = new TreeInstance[original.Length];

			System.Array.Copy(original, treeInst, original.Length);

			return treeInst;
		}
}