# Greenline
Unpacker and Config Extractor for managed Redline Stealer payloads

## How to use

```text
Greenline.exe <path> [--config-only]
```

Greenline will by default unpack Redline Stealers string obfuscation, if you only want the config use the `--config-only` argument after the path to your binary.

## Features

### String deobfuscation

![](https://dr4k0nia.github.io/images/redline_string_dnspy.png)

Greenline will unpack string obfuscation like this back to a readable form like this.

![grafik](https://user-images.githubusercontent.com/51999910/210598568-64359e4f-abd6-43a6-b61d-d3c98b5f6876.png)

### Config extraction

Greenline also automatically extracts the config of RedLine Stealer

![](https://dr4k0nia.github.io/images/extracted_config.png)


## More information
If you want a more detailed explanation of how this tool works check out [my blog post](https://dr4k0nia.github.io/posts/Unpacking-RedLine-Stealer/)
