using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Hout.Models;
using Hout.Models.Device;
using Hout.Models.ParamValidation;
using Hout.Models.Specifications;

namespace Hout.Plugins.LimitlessLED
{
    public class LimitlessLEDWhite : BaseDevice
    {
        private const int Port = 8899;
        private const string COMMAND_NAME_BRIGHTNESS_UP = "Brightness Up";
        private const string COMMAND_NAME_BRIGHTNESS_DOWN = "Brightness Down";
        private const string COMMAND_NAME_SET_BRIGHTNESS = "Set Brightness";
        private const string COMMAND_NAME_TURN_ON = "Turn On";
        private const string COMMAND_NAME_TURN_OFF = "Turn Off";
        private const string COMMAND_NAME_TOGGLE = "Toggle";
        private const string COMMAND_NAME_NIGHT_MODE = "Night Mode";
        private const string COMMAND_NAME_WARMTH_UP = "Warmer";
        private const string COMMAND_NAME_WARMTH_DOWN = "Colder";
        private const string COMMAND_NAME_SET_WARMTH = "Set Warmth";
        public sbyte Group
        {
            get { return (sbyte) Properties["Group"]; }
            set { Properties["Group"] = value; }
        }

        public string Address
        {
            get { return (string) Properties["Address"]; }
            set { Properties["Address"] = value; }
        }
        public string State
        {
            get { return (string) Properties["State"]; }
            set { Properties["State"] = value; }
        }
        public sbyte Brightness
        {
            get { return (sbyte) Properties["Brightness"]; }
            set { Properties["Brightness"] = value; }
        }
        public sbyte Warmth
        {
            get { return (sbyte) Properties["Warmth"]; }
            set { Properties["Warmth"] = value; }
        }
        #region CommandBytes
        private const byte CmdBrightnessUp = 0x3C;
        private const byte CmdBrightnessDown = 0x34;
        private const byte CmdWarmthUp = 0x3C;
        private const byte CmdWarmthDown = 0x34;
        private byte CmdOn
        {
            get
            {
                switch (Group)
                {
                    case 1:
                        return 0x38;
                    case 2:
                        return 0x3D;
                    case 3:
                        return 0x37;
                    case 4:
                        return 0x32;
                    default:
                        return default(byte);
                }
            }
        }
        private byte CmdOff
        {
            get
            {
                switch (Group)
                {
                    case 1:
                        return 0x3B;
                    case 2:
                        return 0x33;
                    case 3:
                        return 0x3A;
                    case 4:
                        return 0x36;
                    default:
                        return default(byte);
                }
            }
        }
        private byte CmdNight
        {
            get
            {
                switch (Group)
                {
                    case 1:
                        return 0xBB;
                    case 2:
                        return 0xB3;
                    case 3:
                        return 0xBA;
                    case 4:
                        return 0xB6;
                    default:
                        return default(byte);
                }
            }
        }
        private byte CmdMaxBrightness
        {
            get
            {
                switch (Group)
                {
                    case 1:
                        return 0xB8;
                    case 2:
                        return 0xBD;
                    case 3:
                        return 0xB7;
                    case 4:
                        return 0xB2;
                    default:
                        return default(byte);
                }
            }
        }
        #endregion CommandBytes
        public override string Id { get; }
        public override string Name { get; }
        public override string IconPath { get; }
        #region Specifications
        private static NameDescCollection<PropertySpecification> _propertySpecifications;

        public override NameDescCollection<PropertySpecification> PropertySpecifications
            => _propertySpecifications ?? (_propertySpecifications = new NameDescCollection<PropertySpecification>
            {
                new PropertySpecification
                {
                    Name = "Address",
                    Description = "The IP address/domain name of the LimitlessLED bridge.",
                    DefaultValue = "",
                    ReadOnly = false,
                    Hidden = false,
                    Type = typeof (string)
                },
                new PropertySpecification
                {
                    Name = "Group",
                    Description = "In which group is the bulb on the bridge? 1-4",
                    DefaultValue = 1,
                    ReadOnly = true,
                    Hidden = false,
                    Type = typeof (sbyte)
                },
                new PropertySpecification
                {
                    Name = "State",
                    Description = "The current state of the light bulb.",
                    DefaultValue = "Off",
                    ReadOnly = true,
                    Hidden = false,
                    Type = typeof (string)
                },
                new PropertySpecification
                {
                    Name = "Brightness",
                    Description = "The current warmth of the bulb",
                    DefaultValue = 10,
                    ReadOnly = true,
                    Hidden = true,
                    Type = typeof(sbyte)
                },
                new PropertySpecification
                {
                    Name = "Warmth",
                    Description = "The current warmth of the bulb",
                    DefaultValue = 10,
                    ReadOnly = true,
                    Hidden = true,
                    Type = typeof(sbyte)
                }
            });
        private static NameDescCollection<EventSpecification> _eventSpecifications;

        public override NameDescCollection<EventSpecification> EventSpecifications
            => _eventSpecifications ?? (_eventSpecifications = new NameDescCollection<EventSpecification>
            {
                new EventSpecification
                {
                    Name = "On",
                    Description = "Will fire when the bulb turns on."
                },
                new EventSpecification
                {
                    Name = "Off",
                    Description = "Will fire when the bulb turns off."
                },
                new EventSpecification
                {
                    Name = "Night",
                    Description = "Will fire when the bulb goes to night mode."
                },
                new EventSpecification
                {
                    Name = "StateChanged",
                    Description = "Will fire when the state of the bulb changes.",
                    ParameterSpecifications = new NameDescCollection<ParameterSpecification>
                    {
                        new ParameterSpecification
                        {
                            Name = "Old State",
                            Description = "The old state of the bulb.",
                            Type = typeof(string)
                        },
                        new ParameterSpecification
                        {
                            Name = "New State",
                            Description = "The new state of the bulb.",
                            Type = typeof(string)
                        }
                    }
                }
            });
        private static NameDescCollection<CommandSpecification> _commandSpecifications;

        public override NameDescCollection<CommandSpecification> CommandSpecifications
            => _commandSpecifications ?? (_commandSpecifications = new NameDescCollection<CommandSpecification>
            {
                new CommandSpecification
                {
                    Name = COMMAND_NAME_BRIGHTNESS_UP,
                    Description = "Increases the brightness by 1 level."
                },
                new CommandSpecification
                {
                    Name = COMMAND_NAME_BRIGHTNESS_DOWN,
                    Description = "Decreases the brightness by 1 level."
                },
                new CommandSpecification
                {
                    Name = COMMAND_NAME_SET_BRIGHTNESS,
                    Description = "Sets the brightness to a specific value.",
                    ParameterSpecifications = new NameDescCollection<ParameterSpecification>
                    {
                        new ParameterSpecification
                        {
                            Name = "Value",
                            Description = "The new brightness value (between 1 and 10).",
                            Type = typeof(sbyte),
                            Validator = new NumberValidator(1, 10)
                        }
                    }
                },
                new CommandSpecification
                {
                    Name = COMMAND_NAME_TURN_ON,
                    Description = "Turns the light bulb on."
                },
                new CommandSpecification
                {
                    Name = COMMAND_NAME_TURN_OFF,
                    Description = "Turns the light bulb off."
                },
                new CommandSpecification
                {
                    Name = COMMAND_NAME_TOGGLE,
                    Description = "Toggles the light bulb on and off."
                },
                new CommandSpecification
                {
                    Name = COMMAND_NAME_NIGHT_MODE,
                    Description = "Puts the light bulb in night mode."
                },
                new CommandSpecification
                {
                    Name = COMMAND_NAME_WARMTH_UP,
                    Description = "Makes the light bulb light warmer."
                },
                new CommandSpecification
                {
                    Name = COMMAND_NAME_WARMTH_DOWN,
                    Description = "Makes the light bulb light colder."
                },
                new CommandSpecification
                {
                    Name = COMMAND_NAME_SET_WARMTH,
                    Description = "Sets the warmth to a specific value.",
                    ParameterSpecifications = new NameDescCollection<ParameterSpecification>
                    {
                        new ParameterSpecification
                        {
                            Name = "Value",
                            Description = "The new warmth value (between 1 and 10)",
                            Type = typeof(byte),
                            Validator = new NumberValidator(1, 10)
                        }
                    }
                }
            });
        #endregion Specifications
        public override async Task<CommandResponse> ExecuteCommand(string name, Dictionary<string, object> parameters)
        {
            switch (name)
            {
                case COMMAND_NAME_BRIGHTNESS_UP:
                    await SetBrightnessAsync((sbyte) (Brightness + 1));
                    return new CommandResponse();
                case COMMAND_NAME_BRIGHTNESS_DOWN:
                    await SetBrightnessAsync((sbyte)(Brightness + 1));
                    return new CommandResponse();
                case COMMAND_NAME_SET_BRIGHTNESS:
                    var newBrightness = (sbyte) parameters["Value"];
                    await SetBrightnessAsync(newBrightness);
                    return new CommandResponse();
                case COMMAND_NAME_WARMTH_UP:
                    await SetWarmthAsync((sbyte)(Warmth + 1));
                    return new CommandResponse();
                case COMMAND_NAME_WARMTH_DOWN:
                    await SetWarmthAsync((sbyte)(Warmth + 1));
                    return new CommandResponse();
                case COMMAND_NAME_SET_WARMTH:
                    var newWarmth = (sbyte)parameters["Value"];
                    await SetBrightnessAsync(newWarmth);
                    return new CommandResponse();
                case COMMAND_NAME_NIGHT_MODE:
                    await SetState("Night");
                    return new CommandResponse();
                case COMMAND_NAME_TOGGLE:
                    await ToggleOnOff();
                    return new CommandResponse();
                case COMMAND_NAME_TURN_OFF:
                    await SetState("Off");
                    return new CommandResponse();
                case COMMAND_NAME_TURN_ON:
                    await SetState("On");
                    return new CommandResponse();
                default:
                    return new CommandResponse(false, "Command not found");
            }
        }
        private async Task SetMaxBrightnessAsync()
        {
            Brightness = 10;
            await SendBytes(100, CmdOn, CmdMaxBrightness);
        }
        private async Task SendBytes(int delay, params byte[] bytes)
        {
            using (var udpClient = new UdpClient(Address, Port))
            {
                foreach (var b in bytes)
                {
                    await udpClient.SendAsync(new byte[]
                        {
                            b,
                            0x0,
                            0x55
                        }, 3);
                    if (delay > 0)
                        await Task.Delay(delay);
                }
            }
        }
        private async Task SetBrightnessAsync(sbyte brightness)
        {
            if (brightness > 10 || brightness < 1)
                return;
            await SetState("On");
            if (Brightness == brightness)
                return;
            if (brightness == 10)
            {
                await SetMaxBrightnessAsync();
                return;
            }
            var diff = brightness - Brightness;
            var b = diff > 0 ? CmdBrightnessUp : CmdBrightnessDown;
            var bytes = new byte[Math.Abs(diff)];
            for (var i = 0; i < bytes.Length; i++)
                bytes[i] = b;
            Brightness = brightness;
            await SendBytes(150, bytes);
        }
        private async Task SetWarmthAsync(sbyte warmth)
        {
            if (warmth > 10 || warmth < 1)
                return;
            await SetState("On");
            var currWarmth = Warmth;
            if (currWarmth == warmth)
                return;
            var diff = warmth - currWarmth;
            var b = diff > 0 ? CmdWarmthUp : CmdWarmthDown;
            var bytes = new byte[Math.Abs(diff)];
            for (var i = 0; i < bytes.Length; i++)
                bytes[i] = b;
            Warmth = warmth;
            await SendBytes(150, bytes);
        }
        private async Task ToggleOnOff()
        {
            if (State != "Off")
                await SetState("Off");
            else
                await SetState("On");
        }
        private async Task SetState(string state)
        {
            switch (state)
            {
                case "On":
                    await SendBytes(150, CmdOn);
                    break;
                case "Off":
                    await SendBytes(150, CmdOff);
                    break;
                case "Night":
                    await SendBytes(150, CmdNight);
                    break;
                default:
                    throw new Exception("Invalid state: " + state);
            }
            State = state;
        }
    }
}
