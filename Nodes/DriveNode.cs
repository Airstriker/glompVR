using System;

namespace glomp
{
	public class DriveNode : DirectoryNode
	{
		public DriveNode (String _fileName)
			: base(_fileName) {
			type = Node.NodeType.DRIVE_NODE;
			IsDrive = true;
		}
	}
}

