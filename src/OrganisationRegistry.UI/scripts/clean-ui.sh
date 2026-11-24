#!/bin/sh
set -e

remove_build_directory_safely() {
    local dir_path="$1"
    
    if [ -d "$dir_path" ]; then
        echo "Removing: $dir_path"
        rm -rf "$dir_path" 2>/dev/null || {
            echo "Warning: Could not remove $dir_path, trying with sudo..."
            sudo rm -rf "$dir_path" 2>/dev/null || {
                echo "Error: Failed to remove $dir_path"
                return 1
            }
        }
    else
        echo "Directory $dir_path does not exist, skipping"
    fi
}

remove_build_directory_safely "dist"
remove_build_directory_safely "wwwroot"

mkdir -p wwwroot

echo "Clean completed successfully"