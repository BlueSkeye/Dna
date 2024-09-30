using DotNetGraph.Attributes;
using DotNetGraph.Core;

namespace Dna.ControlFlow
{
    public static class GraphVisualizer
    {
        /// <summary>Gets a graphviz representation of the control flow graph.</summary>
        public static DotGraph GetDotGraph<T>(ControlFlowGraph<T> graph)
        {
            // Create a dotviz block for each basic block.
            // TODO : Unclear wether the true value is for Directed or Strict property.
            DotGraph dotGraph = new DotGraph() {
                Identifier = new DotIdentifier(graph.Name),
                Directed = true
            };
            Dictionary<BasicBlock<T>, DotNode> blockMapping = new Dictionary<BasicBlock<T>, DotNode>();
            foreach(BasicBlock<T> block in graph.GetBlocks()) {
                DotNode node = GetDotNode(block);
                blockMapping.Add(block, node);
                dotGraph.Elements.Add(node);
            }
            // Create dotviz edges between basic blocks.
            foreach(BasicBlock<T> block in graph.GetBlocks()) {
                List<BlockEdge<T>> outgoingEdges = block.GetOutgoingEdges().ToList();
                for(int i = 0; i < outgoingEdges.Count; i++) {
                    BlockEdge<T> blockEdge = outgoingEdges[i];
                    DotColor lineColor = GetEdgeColor(i, outgoingEdges.Count);
                    DotEdge dotEdge = GetDotEdge(blockMapping[blockEdge.TargetBlock],
                        blockMapping[blockEdge.SourceBlock], lineColor);
                    dotGraph.Elements.Add(dotEdge);
                }
            }
            return dotGraph;
        }

        private static DotNode GetDotNode<T>(BasicBlock<T> block)
        {
            return new DotNode() {
                Color = new DotColor(255, 66, 66, 66),
                FontColor = DotColor.Black,
                Identifier = new DotIdentifier(block.Name),
                Label = GraphFormatter.FormatBlock(block),
                Shape = DotNodeShape.Box,
            };
        }

        /// <summary>Gets a line color for the provided edge.</summary>
        private static DotColor GetEdgeColor(int index, int count)
        {
            DotColor thenColor = DotColor.Green;
            DotColor elseColor = DotColor.Red;
            DotColor switchColor = DotColor.LightBlue;

            if (count > 2) {
                return switchColor;
            }
            return (index == 0) ? thenColor : elseColor;
        }

        private static DotEdge GetDotEdge(DotNode from, DotNode to, DotColorAttribute lineColor)
        {
            return new DotEdge() {
                ArrowHead = DotEdgeArrowType.Normal,
                ArrowTail = DotEdgeArrowType.None,
                Color = lineColor,
                From = from.Identifier,
                Label = string.Empty,
                Style = DotEdgeStyle.Solid,
                To = to.Identifier
            };
        }
    }
}
