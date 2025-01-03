# DS3DXMLImporter

Welcome to **DS3DXMLImporter**! This project provides a robust solution for importing and parsing 3DXML files (V5 and V6) directly into Unity. With its advanced features and performance-focused design, the importer ensures seamless integration of 3DXML content into Unity projects.

---

## 🚀 Features

- **Support for 3DXML V5 and V6**:
  - Fully compatible with both versions of the 3DXML format.

- **Comprehensive Data Extraction**:
  - Loads all essential elements, including:
    - Product structures
    - Geometric elements
    - Colors and material information
    - Normals and surface attributes

- **Matrix Conversion**:
  - Accurate conversion of transformation matrices for precise placement and orientation of objects.

- **Optimized Performance**:
  - Cutting-edge optimizations for:
    - Efficient parsing of XML data.
    - Reduced memory allocations.
    - Multi-threaded processing for faster loading of large datasets.

---

## 📖 How It Works

1. **Parse the 3DXML File**:
   - The loader parses the 3DXML archive to extract its content.
   - Handles both embedded and external references seamlessly.

2. **Extract and Process Data**:
   - Reads the manifest to identify references and structures.
   - Converts geometry data (vertices, normals, colors) into Unity's native mesh format.

3. **Create Unity Objects**:
   - Generates Unity GameObjects with meshes, materials, and hierarchies matching the original 3DXML data.

4. **Optimized Integration**:
   - Minimizes memory overhead and maximizes frame-rate stability during runtime.

---

## 🛠️ Installation

1. Clone this repository:
   ```bash
   git clone https://github.com/yourusername/DS3DXMLImporter.git

2. Open your Unity project and add the importer as a package or directly include the `Assets/DS3DXMLImporter` folder.

---

## 💻 Usage

After adding the importer package, you can begin parsing and loading 3DXML files into Unity with the **DS3DXMLParser**.

---

## 🏎️ Performance Optimizations

- **Preallocated Data Structures**:
  - Reduces unnecessary dynamic resizing of collections.

- **Multi-threaded Parsing**:
  - Leveraging multi-core CPUs for faster processing.

- **Efficient Memory Management**:
  - Ensures minimal garbage collection overhead during runtime.

- **Optimized Geometry Processing**:
  - Uses batching and streamlined workflows to handle large datasets effectively.

---

## 🛡️ Limitations

- Currently, this loader supports most standard 3DXML features but may not cover all edge cases or custom extensions.
- Large files with highly detailed geometry may take longer to process due to their complexity.

---

## 🛠️ Contributing

We welcome contributions! If you find bugs or have ideas for improvements, feel free to:
- Submit an issue.
- Fork the repository and open a pull request.

---

## 📜 License

This project is licensed under the [MIT License](LICENSE).

---

## 🙌 Acknowledgments

Special thanks to:
- [Dassault Systèmes](https://www.3ds.com/) for the 3DXML format.
- The Unity community for their support and tools that make projects like this possible.

---

## 🤝 Contact

For questions or suggestions, feel free to reach out:
- GitHub Issues: [Submit an issue](https://github.com/ReikanYsora/DS3DXMLImporter/issues)
- Email: [your.email@example.com](mailto:jcremoux@gmail.com)

---

## 🎮 Unity Version Support

This project is compatible with **Unity 6** and is designed to provide optimal performance when used with this version of Unity.

---

Happy Loading! 🎉
